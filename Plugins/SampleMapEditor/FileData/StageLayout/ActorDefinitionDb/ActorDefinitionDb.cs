using ByamlExt.Byaml;
using CafeLibrary;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io;
using Wheatley.io.BYML;
//using static BrawlLib.Modeling.Triangle_Converter.GraphArray<T>;
using static System.Net.WebRequestMethods;

namespace SampleMapEditor
{
    public class ActorDefinitionDb
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        private BymlFileData BymlData;

        public ActorDefinitionDb()
        {
            BymlData = new BymlFileData()
            {
                byteOrder = Syroot.BinaryData.ByteOrder.LittleEndian,
                SupportPaths = false,
                Version = 3,
            };
        }


        public ActorDefinitionDb(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Load(stream);
            }
        }


        public ActorDefinitionDb(Stream stream)
        {
            Load(stream);
        }


        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets or sets the list of <see cref="ActorDefinition"/> instances in this database.
        /// </summary>
        public List<ActorDefinition> Definitions
        {
            get;
            set;
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Saves the definitions into the file with the given name.
        /// </summary>
        /// <param name="fileName">The name of the file in which the definitions will be stored.</param>
        public void Save(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(stream);
            }
        }

        /// <summary>
        /// Saves the definitions into the the given stream.
        /// </summary>
        /// <param name="stream">The stream in which the definitions will be stored.</param>
        public void Save(Stream stream)     // ???
        {
            BymlData.RootNode = ByamlSerialize.Serialize(this);
            ByamlFile.SaveN(stream, BymlData);
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static string FindFilePath(string resName)
        {
            string resFilePath = GlobalSettings.GetContentPath($"Pack\\Actor\\{resName}.pack.zs");

            if (System.IO.File.Exists(resFilePath)) return resFilePath;

            return "";
        }

        private void Load(Stream stream)
        {
            //BymlData = ByamlFile.LoadN(stream, true);
            //ByamlSerialize.Deserialize(this, BymlData.RootNode);
            BinaryDataReader br = new BinaryDataReader(stream, Encoding.UTF8, false);
            uint ZSTDMagic = br.ReadUInt32();
            br.Position = 0;

            if (ZSTDMagic == 0xFD2FB528)
            {
                ZstdNet.Decompressor Dec = new ZstdNet.Decompressor();
                byte[] res = Dec.Unwrap(br.ReadBytes((int)br.Length));

                MemoryStream stream1 = new MemoryStream();
                stream1.Write(res, 0, res.Length);
                stream1.Position = 0;
                Load(stream1);
                return;
            }

            BymlData = ByamlFile.LoadN(stream, true);

            ByamlSerialize.Deserialize(this, BymlData.RootNode);

            // Trying to fix junks
            for (int i = 0; i < Definitions.Count; i++)
            {
                // Setting these parameters to "" instead of null
                Definitions[i].FmdbName = "";
                Definitions[i].JmpName = "";
                Definitions[i].LinkUserName = "";
                Definitions[i].ParamsFileBaseName = "";
                Definitions[i].ResJmpName = "";
                Definitions[i].ResName = "";

                // In Splatoon 3, Name is __RowId
                Definitions[i].Name = Definitions[i].__RowId;

                // Trying to extract ResName and FmdbName out of Fmdb

                // If the path is empty, then there's no point in extracting stuff
                if (Definitions[i].Fmdb == "")
                    continue;

                // Split the path by '/'
                // It goes like this: FldObj_Hiagari03/output/FldObj_Hiagari03YaguraFloat.fmdb
                // Here: ResName  = FldObj_Hiagari03
                //       FmbdName = FldObj_Hiagari03YaguraFloat

                string[] words = Definitions[i].Fmdb.Split('/');

                Definitions[i].FmdbName = Path.GetFileNameWithoutExtension(words[words.Length - 1]);
                Definitions[i].ResName  = words[words.Length - 3];

                // Load sub models
                if (FindFilePath(Definitions[i].Name) != "")
                {
                    // Account for the ZStandard compression
                    Stream sstream = System.IO.File.OpenRead(FindFilePath(Definitions[i].Name));
                    BinaryDataReader bbr = new BinaryDataReader(sstream, Encoding.UTF8, false);
                    ZSTDMagic = bbr.ReadUInt32();
                    bbr.Position = 0;

                    if (ZSTDMagic == 0xFD2FB528)
                    {
                        ZstdNet.Decompressor Dec = new ZstdNet.Decompressor();
                        byte[] res = Dec.Unwrap(bbr.ReadBytes((int)bbr.Length));

                        MemoryStream stream1 = new MemoryStream();
                        stream1.Write(res, 0, res.Length);
                        stream1.Position = 0;

                        sstream = stream1;
                    }

                    SARC arc = new SARC();
                    arc.Load(sstream);


                    ArchiveFileInfo SerializedModelInfoFile = null;
                    for (int f = 0; f < arc.files.Count; f++)
                    {
                        string[] splitPath = arc.files[f].FileName.Split('/');

                        if (splitPath.Length > 2 && splitPath[0] == "Component" && splitPath[1] == "ModelInfo")
                        {
                            SerializedModelInfoFile = arc.files[f];
                        }
                    }

                    // Exploit file
                    if (SerializedModelInfoFile != null)
                    {
                        BinaryDataReader bbbr = new BinaryDataReader(SerializedModelInfoFile.FileData, Encoding.UTF8, false);
                        BYML mData = new BYML(new NitroFile(bbbr.ReadBytes((int)bbbr.Length)));

                        BymlNode SubModels = mData.Root["SubModels"];

                        if (SubModels != null)
                        {
                            foreach (BymlNode.DictionaryNode SubModel in SubModels.Nodes)
                            {
                                BymlNode SerializedFmbd = SubModel["Fmdb"];

                                if (SerializedFmbd != null)
                                {
                                    string SubFmbd = Path.GetFileNameWithoutExtension(((BymlNode.String)SerializedFmbd).Text);
                                    Definitions[i].SubModels.Add(SubFmbd);
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Finished loading ActorDb.");
        }
    }
}
