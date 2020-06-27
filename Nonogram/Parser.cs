using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nonogram
{

    static class Parser
    {
        /// <summary>
        /// Convert a puzzle to a json format and write it
        /// </summary>
        /// <param name="puzzle">The puzzle to convert</param>
        /// <param name="textWriter">Output writer</param>
        public static void PuzzleToJson(Puzzle puzzle, TextWriter textWriter)
        {
            textWriter.Write("{\"Puzzle\":{");
            // Puzzle width
            textWriter.Write("\"SizeX\":\"" + puzzle.SizeX.ToString() + "\",");
            // Puzzle height
            textWriter.Write("\"SizeY\":\"" + puzzle.SizeY.ToString() + "\",");
            // Horizontal checksum
            JsonChecksumCollection(puzzle.Horizontal, textWriter, "Horizontal");
            textWriter.Write(",");
            // Vertical checksum
            JsonChecksumCollection(puzzle.Vertical, textWriter, "Vertical");
            textWriter.Write(",");
            // Puzzle state
            WriteJsonState(puzzle, textWriter);

            
            textWriter.Write("}}");
        }
        /// <summary>
        /// Writes state of a puzzle in the Json format
        /// </summary>
        /// <param name="puzzle">Puzzle</param>
        /// <param name="textWriter">Writer</param>
        private static void WriteJsonState(Puzzle puzzle, TextWriter textWriter)
        {
            textWriter.Write("\"State\":[");
            for (int i = 0; i < puzzle.SizeX; i++)
            {
                for (int j = 0; j < puzzle.SizeY; j++)
                {
                    textWriter.Write((int)puzzle[i,j].State);
                    if (i < puzzle.SizeX - 1 || j < puzzle.SizeY - 1)
                    {
                        textWriter.Write(",");
                    }
                }
            }
            textWriter.Write("]");
        }

        /// <summary>
        /// Convert a puzzle to an xml format and write it
        /// </summary>
        /// <param name="puzzle">The puzzle to convert</param>
        /// <param name="textWriter">Output writer</param>
        public static void PuzzleToXml(Puzzle puzzle, TextWriter textWriter)
        {
            // XML version
            textWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf - 8\"?>");

            textWriter.WriteLine("<Puzzle>");
            // Puzzle width
            textWriter.WriteLine("<SizeX>" + puzzle.SizeX.ToString() + "</SizeX>");
            // Puzzle height
            textWriter.WriteLine("<SizeY>" + puzzle.SizeY.ToString() + "</SizeY>");
            // Horizontal checksum
            ChecksumCollectionToXml(puzzle.Horizontal, textWriter, "Horizontal");
            // Vertical checksum
            ChecksumCollectionToXml(puzzle.Vertical, textWriter, "Vertical");
            // PuzzleState
            WriteXmlState(puzzle, textWriter);

            textWriter.WriteLine("</Puzzle>");
        }
        /// <summary>
        /// Writes the state of a puzzle in the Xml format
        /// </summary>
        /// <param name="puzzle">Puzzle</param>
        /// <param name="textWriter">Text writer</param>
        private static void WriteXmlState(Puzzle puzzle, TextWriter textWriter)
        {

            textWriter.WriteLine("<State>");
            for (int i = 0; i < puzzle.SizeX; i++)
            {
                for (int j = 0; j < puzzle.SizeY; j++)
                {
                    textWriter.WriteLine("  <Cell>" + (int)puzzle[i,j].State + "</Cell>" );
                }
            }
            textWriter.WriteLine("</State>");
        }

        /// <summary>
        /// Parse a puzzle in json format
        /// </summary>
        /// <param name="reader">Input reader</param>
        /// <returns>Parsed puzzle</returns>
        public static Puzzle JsonToPuzzle(TextReader reader)
        {
            var jsonReader = new JsonTextReader(reader);
            int sizeX = 0, sizeY = 0;
            CellState[] state = null;
            Puzzle.ChecksumCollection horizontal = null, vertical = null;
            while (jsonReader.Read())
            {
                if (jsonReader.Value != null)
                {
                    switch (jsonReader.Value)
                    {
                        case "SizeX":
                        {
                            sizeX = (int)jsonReader.ReadAsInt32();
                            break;
                        }
                        case "SizeY":
                        {
                            sizeY = (int)jsonReader.ReadAsInt32();
                            break;
                        }
                        case "Horizontal":
                        {
                            horizontal = readJsonChecksum(sizeY, jsonReader);
                            break;
                        }
                        case "Vertical":
                        {
                            vertical = readJsonChecksum(sizeX, jsonReader);
                            break;
                        }
                        case "State":
                        {
                            state = readJsonState(sizeX, sizeY, jsonReader);
                            break;
                        }
                    }
                }
            }
            var puzzle = new Puzzle(horizontal, vertical);
            SetPuzzleState(puzzle, state);
            return puzzle;
        }
        /// <summary>
        /// Sets the state of a puzzle
        /// </summary>
        /// <param name="puzzle">Puzzle to write states to</param>
        /// <param name="state">Flat array of states</param>
        private static void SetPuzzleState(Puzzle puzzle, CellState[] state)
        {
            int index = 0;
            for (int i = 0; i < puzzle.SizeX; i++)
            {
                for (int j = 0; j < puzzle.SizeY; j++)
                {
                    puzzle[i, j].State = state[index];
                    index++;
                }
            }
        }

        /// <summary>
        /// Parse json puzzle state
        /// </summary>
        /// <param name="sizeX">Horizontal puzzle size</param>
        /// <param name="sizeY">Vertical puzzle size</param>
        /// <param name="jsonReader">Json reader</param>
        /// <returns>Parsed puzzle state</returns>
        private static CellState[] readJsonState(int sizeX, int sizeY, JsonTextReader jsonReader)
        {
            var result = new CellState[sizeX * sizeY];
            int index = 0;
            while (jsonReader.Read())
            {
                switch (jsonReader.TokenType)
                {
                    case JsonToken.StartArray:
                    {
                        break;
                    }
                    case JsonToken.EndArray:
                    {
                        return result;
                    }
                    default:
                    {
                        result[index] = (CellState)int.Parse(jsonReader.Value.ToString());
                        index++;
                        break;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Parse a puzzle checksum in json format
        /// </summary>
        /// <param name="reader">Input reader</param>
        /// <returns>Parsed checksum collection</returns>
        private static Puzzle.ChecksumCollection readJsonChecksum(int size, JsonTextReader jsonReader)
        {
            var checksum = new Puzzle.ChecksumCollection(size);
            // Starting index at -1, it will increase to 0 when reading first opening bracket
            var index = 0;
            var startingDepth = jsonReader.Depth;
            while (jsonReader.Read())
            {
                switch (jsonReader.TokenType)
                {
                    case JsonToken.StartArray:
                    {
                        break;
                    }
                    case JsonToken.EndArray:
                    {
                        if (index == size - 1)
                        {
                            return checksum;
                        }
                        index++;
                        break;
                    }
                    default:
                    {
                        checksum[index].Add(int.Parse(jsonReader.Value.ToString()));
                        break;
                    }
                }
            }
            return checksum;
        }


        /// <summary>
        /// Parse a puzzle in Xml format
        /// </summary>
        /// <param name="reader">Input reader</param>
        /// <returns>Parsed puzzle</returns>
        public static Puzzle XmlToPuzzle(TextReader reader)
        {
            var settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            var xmlReader = XmlReader.Create(reader, settings);
            xmlReader.MoveToContent();
            int sizeX = 0, sizeY = 0;
            CellState[] state = null;
            Puzzle.ChecksumCollection horizontal = null, vertical = null;
            while (!xmlReader.EOF)
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {
                        case "SizeX":
                        {
                            sizeX = xmlReader.ReadElementContentAsInt();
                            break;
                        }
                        case "SizeY":
                        {
                            sizeY = xmlReader.ReadElementContentAsInt();
                            break;
                        }
                        case "Horizontal":
                        {
                            horizontal = readXmlChecksum(sizeY, xmlReader.ReadSubtree());
                            break;
                        }
                        case "Vertical":
                        {
                            vertical = readXmlChecksum(sizeX, xmlReader.ReadSubtree());
                            break;
                        }
                        case "State":
                        {
                            state = readXmlState(sizeX, sizeY, xmlReader.ReadSubtree());
                            break;
                        }
                        default:
                        {
                            xmlReader.Read();
                            break;
                        }
                    }
                }
                else
                {
                    xmlReader.Read();
                }
            }
            var puzzle = new Puzzle(horizontal, vertical);
            SetPuzzleState(puzzle, state);
            return puzzle;
        }

        /// <summary>
        /// Parses the state of a puzzle saved in xml format
        /// </summary>
        /// <param name="sizeX">Horizontal puzzle size</param>
        /// <param name="sizeY">Vertical puzzle size</param>
        /// <param name="xmlReader">Xml reader</param>
        /// <returns></returns>
        private static CellState[] readXmlState(int sizeX, int sizeY, XmlReader reader)
        {
            // index will be set to 0 after reading the first checksum
            int index = 0;
            var result = new CellState[sizeX * sizeY];
            reader.MoveToContent();
            while (!reader.EOF)
            {
                if (reader.Name == "Cell" && reader.NodeType == XmlNodeType.Element)
                {
                    result[index] = (CellState)reader.ReadElementContentAsInt();
                    index++;
                    continue;
                }
                reader.Read();
            }
            return result;
        }

        /// <summary>
        /// Parse a checksum in xml format
        /// </summary>
        /// <param name="size">Size of the checksum</param>
        /// <param name="reader">Input reader</param>
        /// <returns>Parsed Checksum collection</returns>
        private static Puzzle.ChecksumCollection readXmlChecksum(int size, XmlReader reader)
        {
            // index will be set to 0 after reading the first checksum
            int index = -1;
            var checksum = new Puzzle.ChecksumCollection(size);
            reader.MoveToContent();
            while (!reader.EOF)
            {
                if (reader.Name == "State" && reader.NodeType == XmlNodeType.Element)
                {
                    checksum[index].Add(reader.ReadElementContentAsInt());
                    continue;
                }
                if (reader.Name == "Checksum" && reader.NodeType == XmlNodeType.Element)
                {
                    index++;
                }
                reader.Read();
            }
            return checksum;
        }

        /// <summary>
        /// Write a checksum collection in a json format
        /// </summary>
        /// <param name="collection">Collection to write</param>
        /// <param name="textWriter">Output writer</param>
        /// <param name="name">Checksum name (usually vertical or horizontal)</param>
        private static void JsonChecksumCollection(Puzzle.ChecksumCollection collection, TextWriter textWriter, string name)
        {
            textWriter.Write("\"" + name + "\":[");
            for (int i = 0; i < collection.Count; i++)
            {
                textWriter.Write("[");
                for (int j = 0; j < collection[i].Count; j++)
                {
                    textWriter.Write("\"" + collection[i][j].ToString() + "\"");
                    if (j != collection[i].Count - 1)
                    {
                        textWriter.Write(",");
                    }
                }
                textWriter.Write("]");
                if (i != collection.Count - 1)
                {
                    textWriter.Write(",");
                }
            }
            textWriter.Write("]");
        }

        /// <summary>
        /// Write a checksum collection in an xml format
        /// </summary>
        /// <param name="collection">Collection to write</param>
        /// <param name="textWriter">Output writer</param>
        /// <param name="name">Checksum name (usually vertical or horizontal)</param>
        private static void ChecksumCollectionToXml(Puzzle.ChecksumCollection collection, TextWriter textWriter, string name)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (textWriter == null) throw new ArgumentNullException(nameof(textWriter));
            if (name == null) throw new ArgumentNullException(nameof(name));
            textWriter.WriteLine("<" + name + ">");
            for (int i = 0; i < collection.Count; i++)
            {
                textWriter.WriteLine("  <Checksum>");
                for (int j = 0; j < collection[i].Count; j++)
                {
                    textWriter.WriteLine("      <State>" + collection[i][j].ToString() + "</State>");
                }
                textWriter.WriteLine("  </Checksum>");
            }
            textWriter.WriteLine("</" + name + ">");
        }
    }
}
