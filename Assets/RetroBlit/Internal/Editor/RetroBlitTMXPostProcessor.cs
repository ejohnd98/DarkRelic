using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

/// <summary>
/// TMX loader, converts TMX files into a RetroBlit binary format for quicker access
/// </summary>
public class RetroBlitTMXPostProcessor : AssetPostprocessor
{
    private const int MAXIMUM_SEGMENT_SIZE = 4096;
    private static bool mAssetRefreshPending = false;

    private static List<PendingLayerWrite> mPendingLayers;

    private enum DataCompression
    {
        UNKNOWN = 0,
        NONE,
        ZLIB,
        GZIP
    }

    private enum DataEncoding
    {
        UNKNOWN = 0,
        XML,
        CSV,
        BASE64
    }

    private enum DependencyType
    {
        NONE,
        TSX,
        TX
    }

    private struct PendingLayerWrite
    {
        public int hash;
        public byte[] buf;
    }

    private static void EditorUpdate()
    {
        if (mAssetRefreshPending)
        {
            AssetDatabase.Refresh();
            mAssetRefreshPending = false;
        }
    }

    private static bool AssetIsTMX(string asset)
    {
        string assetExt = Path.GetExtension(asset).ToLowerInvariant();

        if (assetExt == ".tmx")
        {
            return true;
        }
        else if (assetExt == ".xml")
        {
            // Do a quick and dirty check to see if this xml file looks like a Tiled file. Instead of parsing
            // xml (slow) just read first 3 lines, and check for some telltale strings. If this turns out to be
            // a false positive then parsing the file will fail as well. The point of this quick parse is to
            // weed out the vast majority of non-Tiled xml files.
            var reader = new StreamReader(asset);
            if (reader == null)
            {
                return false;
            }

            string line = string.Empty;
            line += reader.ReadLine();
            line += reader.ReadLine();
            line += reader.ReadLine();

            if (line.Contains("<?xml ") && line.Contains(" tiledversion=\"") && line.Contains("<map "))
            {
                return true;
            }
        }

        return false;
    }

    private static bool AssetIsTSXorTX(string asset, out DependencyType type)
    {
        type = DependencyType.NONE;

        string assetExt = Path.GetExtension(asset).ToLowerInvariant();

        if (assetExt == ".tsx")
        {
            type = DependencyType.TSX;
            return true;
        }
        else if (assetExt == ".tx")
        {
            type = DependencyType.TX;
            return true;
        }
        else if (assetExt == ".xml")
        {
            // Do a quick and dirty check to see if this xml file looks like a TSX or TX file. Instead of parsing
            // xml (slow) just read first 3 lines, and check for some telltale strigs. If this turns out to be
            // a false positive then parsing the file will also fail. The point of this quick parse is to
            // weed out the vast majority of non-Tiled xml files.
            var reader = new StreamReader(asset);
            if (reader == null)
            {
                return false;
            }

            string line = string.Empty;
            line += reader.ReadLine();
            line += reader.ReadLine();
            line += reader.ReadLine();

            if (line.Contains("<?xml ") && !line.Contains("<map "))
            {
                if (line.Contains("<tileset ") && line.Contains("tilewidth=\""))
                {
                    type = DependencyType.TSX;
                    return true;
                }
                else if (line.Contains("<template>") && line.Contains("<object>"))
                {
                    type = DependencyType.TX;
                    return true;
                }
            }
        }

        return false;
    }

    private static Dictionary<string, List<string>> BuildTMXDependencyList(string searchDir, bool scanTSX, bool scanTX, HashSet<string> ignoreList, Dictionary<string, List<string>> list = null)
    {
        Dictionary<string, List<string>> dependencyList;

        if (list == null)
        {
            dependencyList = new Dictionary<string, List<string>>();
        }
        else
        {
            dependencyList = list;
        }

        foreach (string filenameRaw in Directory.GetFiles(searchDir))
        {
            var filename = filenameRaw.Replace('\\', '/');

            if (ignoreList.Contains(filename))
            {
                // The file is on the ignore list, meaning we don't care about it for some reasons (deleted, or already pending re-import)
                continue;
            }

            if (AssetIsTMX(filename))
            {
                var xmlDoc = OpenXML(filename);
                if (xmlDoc == null)
                {
                    continue;
                }

                var mapNodeElements = xmlDoc.GetElementsByTagName("map");
                XmlNode mapNode = mapNodeElements.Count > 0 ? mapNodeElements.Item(0) : null;
                if (mapNode == null)
                {
                    continue;
                }

                // TSX first
                for (int i = 0; i < mapNode.ChildNodes.Count; i++)
                {
                    XmlNode node = mapNode.ChildNodes.Item(i);

                    var nodeName = node.LocalName.ToLowerInvariant();

                    // TSX dependency
                    if (nodeName == "tileset")
                    {
                        if (node.Attributes["source"] != null)
                        {
                            var dependencyFullName = Path.GetDirectoryName(filename) + "/" + node.Attributes["source"].Value;

                            // Resolve the path, removing and relative path bits (eg .. or .)
                            dependencyFullName = Path.GetFullPath(dependencyFullName);
                            dependencyFullName = dependencyFullName.Replace('\\', '/');

                            if (!dependencyList.ContainsKey(dependencyFullName))
                            {
                                dependencyList[dependencyFullName] = new List<string>();
                            }

                            dependencyList[dependencyFullName].Add(filename);

                            if (!dependencyList[dependencyFullName].Contains(filename))
                            {
                                dependencyList[dependencyFullName].Add(filename);
                            }
                        }
                    }
                }

                // TX, they could appear in deeply nested objects, so need to run a query using xpath here
                var objectNodeList = mapNode.SelectNodes("//object");
                for (int i = 0; i < objectNodeList.Count; i++)
                {
                    var node = objectNodeList.Item(i);
                    if (node.Attributes["template"] != null)
                    {
                        var dependencyFullName = Path.GetDirectoryName(filename) + "/" + node.Attributes["template"].Value;

                        // Resolve the path, removing and relative path bits (eg .. or .)
                        dependencyFullName = Path.GetFullPath(dependencyFullName);
                        dependencyFullName = dependencyFullName.Replace('\\', '/');

                        if (!dependencyList.ContainsKey(dependencyFullName))
                        {
                            dependencyList[dependencyFullName] = new List<string>();
                        }

                        if (!dependencyList[dependencyFullName].Contains(filename))
                        {
                            dependencyList[dependencyFullName].Add(filename);
                        }
                    }
                }
            }
        }

        foreach (string d in Directory.GetDirectories(searchDir))
        {
            BuildTMXDependencyList(d, scanTSX, scanTX, ignoreList, dependencyList);
        }

        return dependencyList;
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        var projectRootPath = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length) + "/";

        var dependencyIgnoreList = new HashSet<string>();

        var newTMXFiles = new List<string>();
        var deletedTMXFiles = new List<string>();

        var newTXFiles = new List<string>();
        var deletedTXFiles = new List<string>();

        var newTSXFiles = new List<string>();
        var deletedTSXFiles = new List<string>();

        foreach (var asset in importedAssets)
        {
            if (asset.Contains("RetroBlit-ignore"))
            {
                continue;
            }

            if (AssetIsTMX(asset))
            {
                newTMXFiles.Add(asset);
                dependencyIgnoreList.Add(projectRootPath + asset);
            }
            else
            {
                DependencyType type;

                if (AssetIsTSXorTX(asset, out type))
                {
                    if (type == DependencyType.TSX)
                    {
                        newTSXFiles.Add(asset);
                    }
                    else if (type == DependencyType.TX)
                    {
                        newTXFiles.Add(asset);
                    }
                }
            }
        }

        foreach (var asset in movedAssets)
        {
            if (asset.Contains("RetroBlit-ignore"))
            {
                continue;
            }

            if (AssetIsTMX(asset))
            {
                newTMXFiles.Add(asset);
                dependencyIgnoreList.Add(projectRootPath + asset);
            }
            else
            {
                DependencyType type;

                if (AssetIsTSXorTX(asset, out type))
                {
                    if (type == DependencyType.TSX)
                    {
                        newTSXFiles.Add(asset);
                    }
                    else if (type == DependencyType.TX)
                    {
                        newTXFiles.Add(asset);
                    }
                }
            }
        }

        // Not too sure about this, maybe we should leave the generated assets even if the base .tmx file is deleted
        foreach (var asset in deletedAssets)
        {
            if (asset.Contains("RetroBlit-ignore"))
            {
                continue;
            }

            if (AssetIsTMX(asset))
            {
                deletedTMXFiles.Add(asset);
                dependencyIgnoreList.Add(projectRootPath + asset);
            }
            else
            {
                DependencyType type;

                if (AssetIsTSXorTX(asset, out type))
                {
                    if (type == DependencyType.TSX)
                    {
                        deletedTSXFiles.Add(asset);
                    }
                    else if (type == DependencyType.TX)
                    {
                        deletedTXFiles.Add(asset);
                    }
                }
            }
        }

        foreach (var asset in movedFromAssetPaths)
        {
            if (asset.Contains("RetroBlit-ignore"))
            {
                continue;
            }

            if (AssetIsTMX(asset))
            {
                deletedTMXFiles.Add(asset);
                dependencyIgnoreList.Add(projectRootPath + asset);
            }
            else
            {
                DependencyType type;

                if (AssetIsTSXorTX(asset, out type))
                {
                    if (type == DependencyType.TSX)
                    {
                        deletedTSXFiles.Add(asset);
                    }
                    else if (type == DependencyType.TX)
                    {
                        deletedTXFiles.Add(asset);
                    }
                }
            }
        }

        if (newTSXFiles.Count != 0 || newTXFiles.Count != 0)
        {
            var dependencyList = BuildTMXDependencyList(Application.dataPath, newTSXFiles.Count != 0, newTXFiles.Count != 0, dependencyIgnoreList);

            foreach (var tsxFile in newTSXFiles)
            {
                var fullName = projectRootPath + tsxFile;
                if (dependencyList.ContainsKey(fullName))
                {
                    foreach (var dependant in dependencyList[fullName])
                    {
                        var relativePath = dependant.Substring(projectRootPath.Length);
                        newTMXFiles.Add(relativePath);
                    }
                }
            }

            foreach (var templateFile in newTXFiles)
            {
                var fullName = projectRootPath + templateFile;
                if (dependencyList.ContainsKey(fullName))
                {
                    foreach (var dependant in dependencyList[fullName])
                    {
                        var relativePath = dependant.Substring(projectRootPath.Length);
                        newTMXFiles.Add(relativePath);
                    }
                }
            }
        }

        foreach (var asset in newTMXFiles)
        {
            ProcessTMX(asset);
        }
    }

    private static BinaryWriter CreateAssetWriter()
    {
        var memoryStream = new MemoryStream();
        var writer = new BinaryWriter(memoryStream);

        return writer;
    }

    private static void WriteAsset(TMXDef tmxInfo, string folder, string filename, MemoryStream stream)
    {
        var fileStream = new FileStream(folder + "/" + filename, FileMode.Create, FileAccess.Write, FileShare.None, 65536);
        if (fileStream == null)
        {
            Debug.LogError("TMX Importer: Can't create parsed TMX binary file for " + tmxInfo.tmxFileName);
        }

        var data = stream.ToArray();

        fileStream.Write(data, 0, data.Length);
        fileStream.Flush();
        fileStream.Close();

        mAssetRefreshPending = true;
    }

    private static void FlushPendingLayers(string folder)
    {
        var fileStream = new FileStream(folder + "/layers.bytes", FileMode.Create, FileAccess.Write, FileShare.None, 65536);
        if (fileStream == null)
        {
            Debug.LogError("TMX Importer: Can't create parsed TMX binary file for layers");
        }

        BinaryWriter bw = new BinaryWriter(fileStream);

        bw.Write((int)mPendingLayers.Count);

        // Write lookup table place holder
        for (int i = 0; i < mPendingLayers.Count; i++)
        {
            bw.Write(mPendingLayers[i].hash);
            bw.Write((int)0); // Offset placeholder
            bw.Write((int)0); // Size placeholder
        }

        List<int> offsets = new List<int>();

        // Write the data
        for (int i = 0; i < mPendingLayers.Count; i++)
        {
            offsets.Add((int)bw.BaseStream.Position);
            bw.Write(mPendingLayers[i].buf);
        }

        bw.Flush();

        bw.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

        // Write lookup table final
        for (int i = 0; i < mPendingLayers.Count; i++)
        {
            bw.Write(mPendingLayers[i].hash);
            bw.Write((int)offsets[i]);
            bw.Write((int)mPendingLayers[i].buf.Length);
        }

        bw.Flush();
        bw.Close();

        mPendingLayers.Clear();
    }

    private static XmlDocument OpenXML(string filename)
    {
        string tmxContents = File.ReadAllText(filename);

        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.LoadXml(tmxContents);
        }
        catch (XmlException e)
        {
            Debug.LogError("TMX Importer: Can't parse " + filename + ", " + e.ToString());
            return null;
        }

        return xmlDoc;
    }

    private static TMXPropertiesDef ReadPropertiesIfAvailable(string filename, XmlNode parentNode)
    {
        var propsNode = GetFirstChildNode(parentNode, "properties");

        // If there are properties then read and save them
        if (propsNode != null)
        {
            return ReadProperties(filename, propsNode);
        }

        return null;
    }

    private static void WritePropertiesIfAvailable(string filename, XmlNode parentNode, BinaryWriter writer)
    {
        var props = ReadPropertiesIfAvailable(filename, parentNode);
        if (props != null)
        {
            writer.Write(true);
            WriteProperties(writer, props);
        }
        else
        {
            writer.Write(false);
        }
    }

    private static bool ProcessTMXLayer(XmlNode layerNode, TMXDef tmxInfo, BinaryWriter writer, string folder, out string tmxLayerName, GroupSettings groupSettings)
    {
        tmxLayerName = string.Empty;

        if (layerNode == null || layerNode.Attributes["name"] == null)
        {
            Debug.LogError("TMX Importer: Parsing error, layer has no name in file " + tmxInfo.tmxFileName);
            return false;
        }

        tmxLayerName = layerNode.Attributes["name"].Value;

        int layerWidth, layerHeight;
        if (!int.TryParse(layerNode.Attributes["width"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out layerWidth) || !int.TryParse(layerNode.Attributes["height"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out layerHeight))
        {
            Debug.LogError("TMX Importer: Parsing error, layer \"" + tmxLayerName + "\" has invalid or missing dimensions in file " + tmxInfo.tmxFileName);
            return false;
        }

        if (layerWidth < 0 || layerHeight < 0)
        {
            Debug.LogError("TMX Importer: Parsing error, layer \"" + tmxLayerName + "\" has invalid dimensions in file " + tmxInfo.tmxFileName);
            return false;
        }

        float layerOffsetX = 0;
        float layerOffsetY = 0;

        if (layerNode.Attributes["offsetx"] != null)
        {
            if (!float.TryParse(layerNode.Attributes["offsetx"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out layerOffsetX))
            {
                Debug.LogError("TMX Importer: Parsing error, layer \"" + tmxLayerName + "\" has invalid offset in file " + tmxInfo.tmxFileName);
                return false;
            }
        }

        if (layerNode.Attributes["offsety"] != null)
        {
            if (!float.TryParse(layerNode.Attributes["offsety"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out layerOffsetY))
            {
                Debug.LogError("TMX Importer: Parsing error, layer \"" + tmxLayerName + "\" has invalid offset in file " + tmxInfo.tmxFileName);
                return false;
            }
        }

        float layerOpacity = 1.0f;

        if (layerNode.Attributes["opacity"] != null)
        {
            if (!float.TryParse(layerNode.Attributes["opacity"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out layerOpacity))
            {
                Debug.LogError("TMX Importer: Parsing error, layer \"" + tmxLayerName + "\" has invalid opacity in file " + tmxInfo.tmxFileName);
                return false;
            }
        }

        if (layerOpacity < 0)
        {
            layerOpacity = 0;
        }

        if (layerOpacity > 1)
        {
            layerOpacity = 1;
        }

        int layerVisible = 1;
        if (layerNode.Attributes["visible"] != null)
        {
            if (!int.TryParse(layerNode.Attributes["visible"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out layerVisible))
            {
                Debug.LogError("TMX Importer: Parsing error, layer \"" + tmxLayerName + "\" has invalid visibility in file " + tmxInfo.tmxFileName);
                return false;
            }
        }

        if (tmxInfo.infinite)
        {
            var infiniteLayerSize = CalculateInfiniteMapSizeAtNode(tmxInfo.tmxFileName, tmxInfo.chunkSize, layerNode, ".//chunk");
            layerWidth = infiniteLayerSize.x;
            layerHeight = infiniteLayerSize.y;
        }

        writer.Write(tmxLayerName);
        writer.Write(layerWidth);
        writer.Write(layerHeight);
        writer.Write((int)layerOffsetX + groupSettings.offset.x);
        writer.Write((int)layerOffsetY + groupSettings.offset.y);
        writer.Write((layerVisible == 0 || groupSettings.visible == false) ? false : true);
        writer.Write((byte)(layerOpacity * groupSettings.opacity * 255));

        WritePropertiesIfAvailable(tmxInfo.tmxFileName, layerNode, writer);

        FastString mWorkStr = new FastString(2048);
        var tmxLayerNameHash = mWorkStr.Set(tmxLayerName).ToLowerInvariant().GetHashCode();
        var tmxLayerNameHashStr = tmxLayerNameHash.ToString("x");

        for (int i = 0; i < layerNode.ChildNodes.Count; i++)
        {
            XmlNode node = layerNode.ChildNodes.Item(i);
            if (node != null && node.LocalName.ToLowerInvariant() == "data")
            {
                var encodingAttrib = node.Attributes["encoding"];
                var compressionAttrib = node.Attributes["compression"];

                DataEncoding dataEncoding = DataEncoding.XML;
                if (encodingAttrib != null)
                {
                    if (encodingAttrib.Value.ToLowerInvariant() == "xml")
                    {
                        dataEncoding = DataEncoding.XML;
                    }
                    else if (encodingAttrib.Value.ToLowerInvariant() == "csv")
                    {
                        dataEncoding = DataEncoding.CSV;
                    }
                    else if (encodingAttrib.Value.ToLowerInvariant() == "base64")
                    {
                        dataEncoding = DataEncoding.BASE64;
                    }
                    else
                    {
                        dataEncoding = DataEncoding.UNKNOWN;
                        Debug.LogError("TMX Importer: Data encoding \"" + encodingAttrib.Value + "\" is not supported. Try xml, csv, or base64 encoding for file " + tmxInfo.tmxFileName);
                        return false;
                    }
                }

                DataCompression dataCompression = DataCompression.NONE;
                if (compressionAttrib != null)
                {
                    if (compressionAttrib.Value.ToLowerInvariant() == "none")
                    {
                        dataCompression = DataCompression.NONE;
                    }
                    else if (compressionAttrib.Value.ToLowerInvariant() == "zlib")
                    {
                        dataCompression = DataCompression.ZLIB;
                    }
                    else if (compressionAttrib.Value.ToLowerInvariant() == "gzip")
                    {
                        dataCompression = DataCompression.GZIP;
                    }
                    else
                    {
                        dataCompression = DataCompression.UNKNOWN;
                        Debug.LogError("TMX Importer: Data compression \"" + compressionAttrib.Value + "\" is not supported. Try gzip, zlib, or no compression for file " + tmxInfo.tmxFileName);
                        return false;
                    }
                }

                if (tmxInfo.infinite)
                {
                    if (!node.HasChildNodes)
                    {
                        Debug.LogError("TMX Importer: Layer data has no chunks, but the map is infinite in file " + tmxInfo.tmxFileName);
                        return false;
                    }

                    List<Chunk> chunks = new List<Chunk>();

                    for (int j = 0; j < node.ChildNodes.Count; j++)
                    {
                        XmlNode chunkNode = node.ChildNodes.Item(j);
                        if (chunkNode != null && chunkNode.LocalName.ToLowerInvariant() == "chunk")
                        {
                            var chunk = new Chunk();

                            int chunkWidth, chunkHeight;
                            if (chunkNode.Attributes["width"] != null && chunkNode.Attributes["height"] != null)
                            {
                                if (!int.TryParse(chunkNode.Attributes["width"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out chunkWidth) ||
                                    !int.TryParse(chunkNode.Attributes["height"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out chunkHeight))
                                {
                                    Debug.LogError("TMX Importer: Parsing error, can't parse width, or height of chunk in file " + tmxInfo.tmxFileName);
                                    return false;
                                }
                            }
                            else
                            {
                                Debug.LogError("TMX Importer: Parsing error, chunk does not have width and height in file " + tmxInfo.tmxFileName);
                                return false;
                            }

                            int chunkOffsetX, chunkOffsetY;
                            if (chunkNode.Attributes["x"] != null && chunkNode.Attributes["y"] != null)
                            {
                                if (!int.TryParse(chunkNode.Attributes["x"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out chunkOffsetX) ||
                                    !int.TryParse(chunkNode.Attributes["y"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out chunkOffsetY))
                                {
                                    Debug.LogError("TMX Importer: Parsing error, can't parse offset of chunk in file " + tmxInfo.tmxFileName);
                                    return false;
                                }
                            }
                            else
                            {
                                Debug.LogError("TMX Importer: Parsing error, chunk does not have an offset in file " + tmxInfo.tmxFileName);
                                return false;
                            }

                            chunk.offset = new Vector2i(chunkOffsetX, chunkOffsetY);
                            chunk.size = new Vector2i(chunkWidth, chunkHeight);

                            ProcessTMXLayerData(dataEncoding, dataCompression, chunkNode, tmxInfo, chunk.size, chunk.writer);

                            chunks.Add(chunk);
                        }
                    }

                    writer.Write(chunks.Count);

                    int segmentIndex = 0;
                    int segmentOffset = 0;

                    var segmentStream = new MemoryStream();
                    var segmentWriter = new BinaryWriter(segmentStream);

                    var layerDataIndexStream = new MemoryStream();
                    var layerDataIndexWriter = new BinaryWriter(layerDataIndexStream);

                    layerDataIndexWriter.Write(chunks.Count);

                    foreach (var chunk in chunks)
                    {
                        var uncompressedChunkData = chunk.stream.ToArray();
                        var compressedChunkData = RetroBlitInternal.RBDeflate.Compress(uncompressedChunkData);
                        segmentWriter.Write(compressedChunkData, 0, compressedChunkData.Length);

                        ulong part1 = (ulong)chunk.offset.x;
                        ulong part2 = (ulong)chunk.offset.y;
                        ulong offset = ((part1 << 32) & 0xFFFFFFFF00000000) | (part2 & 0xFFFFFFFF);

                        layerDataIndexWriter.Write(offset);
                        layerDataIndexWriter.Write((ushort)segmentIndex);
                        layerDataIndexWriter.Write((ushort)segmentOffset);
                        layerDataIndexWriter.Write((ushort)compressedChunkData.Length);

                        segmentOffset += compressedChunkData.Length;

                        if (segmentOffset >= MAXIMUM_SEGMENT_SIZE)
                        {
                            WriteAsset(tmxInfo, folder, "layer_" + tmxLayerNameHashStr + "_seg_" + segmentIndex + ".bytes", segmentStream);
                            segmentIndex++;

                            segmentStream.Close();
                            segmentWriter.Close();

                            segmentStream = new MemoryStream();
                            segmentWriter = new BinaryWriter(segmentStream);

                            segmentOffset = 0;
                        }
                    }

                    // Write out the last segment
                    if (segmentOffset > 0)
                    {
                        WriteAsset(tmxInfo, folder, "layer_" + tmxLayerNameHashStr + "_seg_" + segmentIndex + ".bytes", segmentStream);
                        segmentIndex++;

                        segmentStream.Close();
                        segmentWriter.Close();
                    }

                    WriteAsset(tmxInfo, folder, "layer_" + tmxLayerNameHashStr + "_index" + ".bytes", layerDataIndexStream);

                    layerDataIndexStream.Close();
                    layerDataIndexWriter.Close();

                    tmxInfo.layerCount++;
                }
                else
                {
                    var layerStream = new MemoryStream();
                    var layerWriter = new BinaryWriter(layerStream);

                    ProcessTMXLayerData(dataEncoding, dataCompression, node, tmxInfo, new Vector2i(layerWidth, layerHeight), layerWriter);

                    var uncompressedLayerData = layerStream.ToArray();
                    var compressedLayerData = RetroBlitInternal.RBDeflate.Compress(uncompressedLayerData);

                    layerWriter.Close();
                    layerStream.Close();

                    layerStream = new MemoryStream();
                    layerWriter = new BinaryWriter(layerStream);

                    layerWriter.Write(compressedLayerData, 0, compressedLayerData.Length);

                    PendingLayerWrite pendingLayerWrite;
                    pendingLayerWrite.hash = tmxLayerNameHash;
                    pendingLayerWrite.buf = layerStream.ToArray();
                    mPendingLayers.Add(pendingLayerWrite);

                    layerWriter.Close();
                    layerStream.Close();

                    tmxInfo.layerCount++;
                }

                break;
            }
        }

        return true;
    }

    private static bool ProcessTMXLayerData(DataEncoding encoding, DataCompression compression, XmlNode dataNode, TMXDef tmxInfo, Vector2i size, BinaryWriter writer)
    {
        if (encoding == DataEncoding.XML)
        {
            if (compression != DataCompression.NONE)
            {
                Debug.LogError("TMX Importer: Compression not valid for xml encoding format in file " + tmxInfo.tmxFileName);
                return false;
            }

            if (!ProcessTMXXMLLayer(dataNode.ChildNodes, size.x * size.y, tmxInfo, writer))
            {
                return false;
            }
        }
        else if (encoding == DataEncoding.CSV)
        {
            if (dataNode.InnerText == null || dataNode.InnerText.Length == 0)
            {
                Debug.LogError("TMX Importer: Data node is empty in file " + tmxInfo.tmxFileName);
                return false;
            }

            if (compression != DataCompression.NONE)
            {
                Debug.LogError("TMX Importer: Compression not valid for csv encoding format in file " + tmxInfo.tmxFileName);
                return false;
            }

            if (!ProcessTMXCSVLayer(dataNode.InnerText, size.x * size.y, tmxInfo, writer))
            {
                return false;
            }
        }
        else if (encoding == DataEncoding.BASE64)
        {
            if (dataNode.InnerText == null || dataNode.InnerText.Length == 0)
            {
                Debug.LogError("TMX Importer: Data node is empty in file " + tmxInfo.tmxFileName);
                return false;
            }

            byte[] base64Data = null;

            if (compression == DataCompression.NONE)
            {
                base64Data = System.Convert.FromBase64String(dataNode.InnerText);
            }
            else if (compression == DataCompression.ZLIB || compression == DataCompression.GZIP)
            {
                base64Data = System.Convert.FromBase64String(dataNode.InnerText);
                try
                {
                    base64Data = RetroBlitInternal.RBDeflate.Decompress(base64Data);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("TMX Importer: Failed to decompress data in file " + tmxInfo.tmxFileName + ", " + e.ToString());
                    return false;
                }
            }

            if (!ProcessTMXBase64Layer(base64Data, size.x * size.y, tmxInfo, writer))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ProcessTMXXMLLayer(XmlNodeList tileNodes, int expectedTileCount, TMXDef tmxInfo, BinaryWriter writer)
    {
        int tileCount = 0;

        for (int i = 0; i < tileNodes.Count; i++)
        {
            XmlNode node = tileNodes.Item(i);
            if (node != null && node.LocalName.ToLowerInvariant() == "tile")
            {
                uint tileId = 0;
                var gidAttrib = node.Attributes["gid"];
                if (gidAttrib != null)
                {
                    if (!uint.TryParse(gidAttrib.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out tileId))
                    {
                        Debug.LogError("TMX Importer: Failed to read a tile id from file " + tmxInfo.tmxFileName);
                    }
                }

                ProcessTile(tileId, tmxInfo, writer);
                tileCount++;
            }
        }

        if (tileCount != expectedTileCount)
        {
            Debug.LogError("TMX Importer: Unexpected amount of tiles in a layer (found " + tileCount + ", expected " + expectedTileCount + ") in file " + tmxInfo.tmxFileName);
            return false;
        }

        return true;
    }

    private static bool ProcessTMXCSVLayer(string csvData, int expectedTileCount, TMXDef tmxInfo, BinaryWriter writer)
    {
        int tileCount = 0;

        if (csvData == null)
        {
            return false;
        }

        char[] delimiterChars = { ' ', ',', '\n', '\r' };
        string[] csvValues = csvData.Split(delimiterChars);

        for (int i = 0; i < csvValues.Length; i++)
        {
            if (csvValues[i] == null)
            {
                continue;
            }

            string csvValue = csvValues[i].Trim();
            if (csvValue == string.Empty)
            {
                continue;
            }

            uint tileInfo;
            if (uint.TryParse(csvValue, NumberStyles.Any, CultureInfo.InvariantCulture, out tileInfo))
            {
                ProcessTile(tileInfo, tmxInfo, writer);
                tileCount++;
            }
        }

        if (tileCount != expectedTileCount)
        {
            Debug.LogError("TMX Importer: Unexpected amount of tiles in a layer (found " + tileCount + ", expected " + expectedTileCount + ") in file " + tmxInfo.tmxFileName);
            return false;
        }

        return true;
    }

    private static bool ProcessTMXBase64Layer(byte[] base64Data, int expectedTileCount, TMXDef tmxInfo, BinaryWriter writer)
    {
        int tileCount = 0;

        if (base64Data == null)
        {
            return false;
        }

        for (int i = 0; i < base64Data.Length; i += 4)
        {
            uint tileInfo = ((uint)base64Data[i + 3] << 24) | ((uint)base64Data[i + 2] << 16) | ((uint)base64Data[i + 1] << 8) | ((uint)base64Data[i]);

            ProcessTile(tileInfo, tmxInfo, writer);
            tileCount++;
        }

        if (tileCount != expectedTileCount)
        {
            Debug.LogError("TMX Importer: Unexpected amount of tiles in a layer (found " + tileCount + ", expected " + expectedTileCount + ") in file " + tmxInfo.tmxFileName);
            return false;
        }

        return true;
    }

    private static void ProcessTile(uint tileInfo, TMXDef tmxInfo, BinaryWriter writer)
    {
        int flags = 0;
        int tmxFlags = (int)((tileInfo & 0xE0000000) >> 29);

        // Translate flags
        switch (tmxFlags)
        {
            case 0: // 000
                flags = 0;
                break;
            case 1: // 001
                // This is not technically valid, TMX never encodes this value, but if its encoded manually this is the result in Tiled editor
                flags = RB.FLIP_V | RB.ROT_90_CW;
                break;
            case 2: // 010
                flags = RB.FLIP_V;
                break;
            case 3: // 011
                flags = RB.ROT_90_CCW;
                break;
            case 4: // 100
                flags = RB.FLIP_H;
                break;
            case 5: // 101
                flags = RB.ROT_90_CW;
                break;
            case 6: // 110
                flags = RB.FLIP_H | RB.FLIP_V;
                break;
            case 7: // 111
                // This is not technically valid, TMX never encodes this value, but if its encoded manually this is the result in Tiled editor
                flags = RB.FLIP_H | RB.ROT_90_CW;
                break;
            default:
                flags = 0;
                break;
        }

        int tileId = (int)(tileInfo & 0x1FFFFFFF);

        // Check for invalid tileid (out of range) and set it to empty
        if (tileId < 0)
        {
            tileId = RB.SPRITE_EMPTY;
        }

        int tsxIndex = 0;

        for (int i = tmxInfo.firstGids.Count - 1; i >= 0; i--)
        {
            if (tmxInfo.firstGids[i] <= tileId)
            {
                tsxIndex = i;
                break;
            }
        }

        tileId -= tmxInfo.firstGids[tsxIndex];
        tileId = tileId < 0 ? RB.SPRITE_EMPTY : tileId;

        writer.Write((byte)(tsxIndex & 0xFF));
        writer.Write((byte)(flags & 0xFF));
        writer.Write(tileId);
    }

    // Process object groups, and recursively process layergroups
    private static bool ProcessTMXLayerAndObjectGroups(XmlNode parentNode, TMXDef tmxInfo, BinaryWriter writer, string folder, GroupSettings groupSettings = null)
    {
        FastString mWorkStr = new FastString(2048);

        if (groupSettings == null)
        {
            groupSettings = new GroupSettings();
        }

        for (int i = 0; i < parentNode.ChildNodes.Count; i++)
        {
            XmlNode node = parentNode.ChildNodes.Item(i);
            if (node.LocalName.ToLowerInvariant() == "layer")
            {
                writer.Write(RetroBlitInternal.RBTilemapTMX.RetroBlit_TMX_SECTION_TILE_LAYER);

                string layerName;
                if (ProcessTMXLayer(node, tmxInfo, writer, folder, out layerName, groupSettings))
                {
                    var layerHash = mWorkStr.Set(layerName).ToLowerInvariant().GetHashCode();

                    if (tmxInfo.layerList.Contains(layerHash))
                    {
                        Debug.LogError("TMX Importer: File has a duplicate layer name \"" + layerName + "\", please use unique layer names in file " + tmxInfo.tmxFileName);
                        return false;
                    }

                    tmxInfo.layerList.Add(layerHash);
                }
            }
            else if (node.LocalName.ToLowerInvariant() == "objectgroup")
            {
                writer.Write(RetroBlitInternal.RBTilemapTMX.RetroBlit_TMX_SECTION_OBJECTGROUP);
                if (!ProcessTMXObjectGroup(tmxInfo, node, writer, groupSettings))
                {
                    return false;
                }
            }

            if (node.LocalName.ToLowerInvariant() == "group")
            {
                // Read group settings, and add them to existing group settings
                float offsetx = 0;
                float offsety = 0;
                float opacity = 1.0f;
                int visible = 1;

                if (node.Attributes["offsetx"] != null)
                {
                    if (!float.TryParse(node.Attributes["offsetx"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out offsetx))
                    {
                        Debug.LogError("TMX Importer: Failed to parse offset x for group in file " + tmxInfo.tmxFileName);
                        return false;
                    }
                }

                if (node.Attributes["offsety"] != null)
                {
                    if (!float.TryParse(node.Attributes["offsety"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out offsety))
                    {
                        Debug.LogError("TMX Importer: Failed to parse offset y for group in file " + tmxInfo.tmxFileName);
                        return false;
                    }
                }

                if (node.Attributes["opacity"] != null)
                {
                    if (!float.TryParse(node.Attributes["opacity"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out opacity))
                    {
                        Debug.LogError("TMX Importer: Failed to parse opacity for group in file " + tmxInfo.tmxFileName);
                        return false;
                    }
                }

                if (node.Attributes["visible"] != null)
                {
                    if (!int.TryParse(node.Attributes["visible"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out visible))
                    {
                        Debug.LogError("TMX Importer: Failed to parse visibility for group in file " + tmxInfo.tmxFileName);
                        return false;
                    }
                }

                groupSettings.offset.x += (int)offsetx;
                groupSettings.offset.y += (int)offsety;
                groupSettings.opacity *= opacity;

                if (groupSettings.visible == true)
                {
                    groupSettings.visible = visible == 0 ? false : true;
                }

                if (!ProcessTMXLayerAndObjectGroups(node, tmxInfo, writer, folder, groupSettings))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static TMXParsedObject GetTemplateObject(TMXDef tmxInfo, string fileName)
    {
        if (tmxInfo.templateObjects.ContainsKey(fileName))
        {
            return tmxInfo.templateObjects[fileName];
        }

        string pathOnly = Path.GetDirectoryName(fileName);
        string fullPath = string.Empty;
        if (Path.IsPathRooted(pathOnly))
        {
            fullPath = fileName;
        }
        else
        {
            fullPath = tmxInfo.relativePath + fileName;
        }

        if (!File.Exists(fullPath))
        {
            Debug.LogError("TMX Importer: File " + tmxInfo.tmxFileName + " references a template TX file " + fileName + " which can't be found.");
            return null;
        }

        var xmlDoc = OpenXML(fullPath);

        if (xmlDoc == null)
        {
            Debug.LogError("TMX Importer: Can't parse TX file " + fileName + ", which is required by " + tmxInfo.tmxFileName);
            return null;
        }

        var templateNodeElements = xmlDoc.GetElementsByTagName("template");
        var templateNode = templateNodeElements.Count > 0 ? templateNodeElements.Item(0) : null;
        if (templateNode == null)
        {
            Debug.LogError("TMX Importer: Can't parse TX file \"" + fileName + "\", <template> not found, this TX file is required by " + tmxInfo.tmxFileName);
            return null;
        }

        var objectNode = GetFirstChildNode(templateNode, "object");

        var obj = ReadTMXObject(tmxInfo, fileName, objectNode, null);

        tmxInfo.templateObjects[fileName] = obj;

        return obj;
    }

    private static bool ProcessTMXObjectGroup(TMXDef tmxInfo, XmlNode objectGroupNode, BinaryWriter writer, GroupSettings groupSettings)
    {
        string objGroupName = null;
        if (objectGroupNode.Attributes["name"] != null)
        {
            objGroupName = objectGroupNode.Attributes["name"].Value;
        }

        if (objGroupName == null)
        {
            Debug.LogError("TMX Importer: ObjectGroup has no name, it can't be referenced without a name, in file " + tmxInfo.tmxFileName);
            return false;
        }

        Color32 objGroupColor = Color.white;
        if (objectGroupNode.Attributes["color"] != null)
        {
            string colorHTML = objectGroupNode.Attributes["color"].Value.Trim();

            if ((colorHTML.Length != 9 && colorHTML.Length != 7) || colorHTML[0] != '#')
            {
                Debug.LogError("TMX Importer: Parsing error, can't parse object group color in file " + tmxInfo.tmxFileName);
                return false;
            }

            uint rgba;
            int r, g, b, a;

            if (!uint.TryParse(
                colorHTML.Substring(1),
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out rgba))
            {
                Debug.LogError("TMX Importer: Parsing error, can't parse object group color in file " + tmxInfo.tmxFileName);
                return false;
            }

            a = 255;

            if (colorHTML.Length == 9)
            {
                a = (int)((rgba & 0xFF000000) >> 24);
            }

            r = (int)((rgba & 0x00FF0000) >> 16);
            g = (int)((rgba & 0x0000FF00) >> 8);
            b = (int)((rgba & 0x000000FF) >> 0);

            objGroupColor = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
        }

        float objGroupOpacity = 1;
        if (objectGroupNode.Attributes["opacity"] != null)
        {
            if (!float.TryParse(objectGroupNode.Attributes["opacity"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out objGroupOpacity))
            {
                Debug.LogError("TMX Importer: ObjectGroup has invalid opacity setting in file " + tmxInfo.tmxFileName);
                return false;
            }

            if (objGroupOpacity < 0)
            {
                objGroupOpacity = 0;
            }
            else if (objGroupOpacity > 1)
            {
                objGroupOpacity = 1;
            }
        }

        int objGroupVisible = 1;
        if (objectGroupNode.Attributes["visible"] != null)
        {
            if (!int.TryParse(objectGroupNode.Attributes["visible"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out objGroupVisible))
            {
                Debug.LogError("TMX Importer: ObjectGroup has invalid opacity setting in file " + tmxInfo.tmxFileName);
                return false;
            }
        }

        float objGroupOffsetX = 0;
        if (objectGroupNode.Attributes["offsetx"] != null)
        {
            if (!float.TryParse(objectGroupNode.Attributes["offsetx"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out objGroupOffsetX))
            {
                Debug.LogError("TMX Importer: ObjectGroup has invalid offset x coordinate in file " + tmxInfo.tmxFileName);
                return false;
            }
        }

        float objGroupOffsetY = 0;
        if (objectGroupNode.Attributes["offsety"] != null)
        {
            if (!float.TryParse(objectGroupNode.Attributes["offsety"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out objGroupOffsetY))
            {
                Debug.LogError("TMX Importer: ObjectGroup has invalid offset y coordinate in file " + tmxInfo.tmxFileName);
                return false;
            }
        }

        writer.Write(objGroupName);
        writer.Write(objGroupColor.r);
        writer.Write(objGroupColor.g);
        writer.Write(objGroupColor.b);
        writer.Write(objGroupColor.a);
        writer.Write((byte)(objGroupOpacity * groupSettings.opacity * 255));
        writer.Write((objGroupVisible == 0 || groupSettings.visible == false) ? false : true);
        writer.Write((int)objGroupOffsetX + groupSettings.offset.x);
        writer.Write((int)objGroupOffsetY + groupSettings.offset.y);

        WritePropertiesIfAvailable(tmxInfo.tmxFileName, objectGroupNode, writer);

        int objectCount = 0;
        for (int k = 0; k < objectGroupNode.ChildNodes.Count; k++)
        {
            XmlNode objectNode = objectGroupNode.ChildNodes.Item(k);
            if (objectNode.LocalName.ToLowerInvariant() == "object")
            {
                objectCount++;
            }
        }

        writer.Write(objectCount);

        for (int k = 0; k < objectGroupNode.ChildNodes.Count; k++)
        {
            XmlNode objectNode = objectGroupNode.ChildNodes.Item(k);
            if (objectNode.LocalName.ToLowerInvariant() != "object")
            {
                continue;
            }

            TMXParsedObject templateObj = null;
            if (objectNode.Attributes["template"] != null)
            {
                string templateFileName = objectNode.Attributes["template"].Value;
                templateObj = GetTemplateObject(tmxInfo, templateFileName);
            }

            var obj = ReadTMXObject(tmxInfo, tmxInfo.tmxFileName, objectNode, templateObj);

            writer.Write(obj.name);
            writer.Write(obj.type);
            writer.Write(obj.rect.x);
            writer.Write(obj.rect.y);
            writer.Write(obj.rect.width);
            writer.Write(obj.rect.height);
            writer.Write(obj.rotation);
            writer.Write(obj.visible);
            writer.Write(obj.tileId);
            writer.Write((int)obj.shape);
            writer.Write(obj.points.Count);

            for (int i = 0; i < obj.points.Count; i++)
            {
                writer.Write(obj.points[i].x);
                writer.Write(obj.points[i].y);
            }

            if (obj.properties != null)
            {
                writer.Write(true);
                WriteProperties(writer, (TMXPropertiesDef)obj.properties);
            }
            else
            {
                writer.Write(false);
            }
        }

        return true;
    }

    private static TMXParsedObject ReadTMXObject(TMXDef tmxInfo, string objectFileName, XmlNode objectNode, TMXParsedObject templateObj)
    {
        string name = string.Empty;
        string type = string.Empty;
        float x = 0;
        float y = 0;
        float width = 0;
        float height = 0;
        float rotation = 0;
        int visible = 1;
        var shape = TMXMapAsset.TMXObject.Shape.Rectangle;
        var points = new List<Vector2i>();
        var tileId = -1;

        // If template obj exists then initialize the values with the template values, then proceed to override any
        // values defined in dervied object
        if (templateObj != null)
        {
            name = templateObj.name;
            type = templateObj.type;
            x = templateObj.rect.x;
            y = templateObj.rect.y;
            width = templateObj.rect.width;
            height = templateObj.rect.height;
            rotation = templateObj.rotation;
            visible = templateObj.visible == true ? 1 : 0;
            shape = templateObj.shape;
            points = templateObj.points;
        }

        if (objectNode.Attributes["name"] != null)
        {
            name = objectNode.Attributes["name"].Value;
        }

        if (objectNode.Attributes["type"] != null)
        {
            type = objectNode.Attributes["type"].Value;
        }

        if (objectNode.Attributes["x"] != null)
        {
            if (!float.TryParse(objectNode.Attributes["x"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out x))
            {
                Debug.LogError("TMX Importer: Parsing error, can't parse x coordinate of object in file " + objectFileName);
                return null;
            }
        }

        if (objectNode.Attributes["y"] != null)
        {
            if (!float.TryParse(objectNode.Attributes["y"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out y))
            {
                Debug.LogError("TMX Importer: Parsing error, can't parse y coordinate of object in file " + objectFileName);
                return null;
            }
        }

        if (objectNode.Attributes["width"] != null)
        {
            if (!float.TryParse(objectNode.Attributes["width"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out width))
            {
                Debug.LogError("TMX Importer: Parsing, can't parse width of object in file " + objectFileName);
                return null;
            }
        }

        if (objectNode.Attributes["height"] != null)
        {
            if (!float.TryParse(objectNode.Attributes["height"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out height))
            {
                Debug.LogError("TMX Importer: Parsing error, can't parse height of object in file " + objectFileName);
                return null;
            }
        }

        var rect = new Rect2i((int)x, (int)y, (int)width, (int)height);

        if (objectNode.Attributes["rotation"] != null)
        {
            if (!float.TryParse(objectNode.Attributes["rotation"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out rotation))
            {
                Debug.LogError("TMX Importer: Parsing error, can't parse rotation of object in file " + objectFileName);
                return null;
            }
        }

        if (objectNode.Attributes["visible"] != null)
        {
            if (!int.TryParse(objectNode.Attributes["visible"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out visible))
            {
                Debug.LogError("TMX Importer: Parsing error, can't parse visibility of object in file " + objectFileName);
                return null;
            }
        }

        int gid = -1;
        if (objectNode.Attributes["gid"] != null)
        {
            if (!int.TryParse(objectNode.Attributes["gid"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out gid))
            {
                Debug.LogError("TMX Importer: Parsing error, can't parse gid (tile id) of object in file " + objectFileName);
                return null;
            }

            tileId = (int)(gid & 0x1FFFFFFF);

            // Check for invalid tileid (out of range) and set it to empty
            if (tileId < 0)
            {
                tileId = RB.SPRITE_EMPTY;
            }

            int tsxIndex = 0;

            for (int i = tmxInfo.firstGids.Count - 1; i >= 0; i--)
            {
                if (tmxInfo.firstGids[i] <= tileId)
                {
                    tsxIndex = i;
                    break;
                }
            }

            tileId -= tmxInfo.firstGids[tsxIndex];
            tileId = tileId < 0 ? RB.SPRITE_EMPTY : tileId;
        }

        for (int i = 0; i < objectNode.ChildNodes.Count; i++)
        {
            XmlNode node = objectNode.ChildNodes.Item(i);
            string nodeName = node.LocalName.ToLowerInvariant();

            if (nodeName == "point")
            {
                shape = TMXMapAsset.TMXObject.Shape.Point;
                break;
            }
            else if (nodeName == "ellipse")
            {
                shape = TMXMapAsset.TMXObject.Shape.Ellipse;
                break;
            }
            else if (nodeName == "rectangle")
            {
                shape = TMXMapAsset.TMXObject.Shape.Rectangle;
                break;
            }
            else if (nodeName == "polygon" || nodeName == "polyline")
            {
                if (nodeName == "polygon")
                {
                    shape = TMXMapAsset.TMXObject.Shape.Polygon;
                }
                else
                {
                    shape = TMXMapAsset.TMXObject.Shape.Polyline;
                }

                if (node.Attributes["points"] != null)
                {
                    var pointStr = node.Attributes["points"].Value;
                    char[] separators = new char[] { ',', ' ' };
                    var nums = pointStr.Split(separators);

                    if (nums.Length % 2 != 0)
                    {
                        Debug.LogError("TMX Importer: Object has an invalid point list, x and y coordinate pairs are not matching in file " + objectFileName);
                        return null;
                    }

                    for (int j = 0; j < nums.Length; j += 2)
                    {
                        float px = 0;
                        if (!float.TryParse(nums[j], NumberStyles.Any, CultureInfo.InvariantCulture, out px))
                        {
                            Debug.LogError("TMX Importer: Object has an invalid x coordinate in it's point list in file " + objectFileName);
                            return null;
                        }

                        float py = 0;
                        if (!float.TryParse(nums[j + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out py))
                        {
                            Debug.LogError("TMX Importer: Object has an invalid x coordinate in it's point list in file " + objectFileName);
                            return null;
                        }

                        points.Add(new Vector2i((int)px, (int)py));
                    }
                }

                break;
            }
        }

        var props = ReadPropertiesIfAvailable(objectFileName, objectNode);

        if (props == null && templateObj != null)
        {
            props = (TMXPropertiesDef)templateObj.properties;
        }
        else if (props != null && templateObj != null && templateObj.properties != null)
        {
            var tprops = (TMXPropertiesDef)templateObj.properties;

            // Merge properties, overriding any duplicates
            if (tprops.GetStrings() != null)
            {
                foreach (var str in tprops.GetStrings())
                {
                    if (!props.GetStrings().ContainsKey(str.Key))
                    {
                        props.GetStrings()[str.Key] = str.Value;
                    }
                }
            }

            if (tprops.GetIntegers() != null)
            {
                foreach (var i in tprops.GetIntegers())
                {
                    if (!props.GetIntegers().ContainsKey(i.Key))
                    {
                        props.GetIntegers()[i.Key] = i.Value;
                    }
                }
            }

            if (tprops.GetFloats() != null)
            {
                foreach (var f in tprops.GetFloats())
                {
                    if (!props.GetFloats().ContainsKey(f.Key))
                    {
                        props.GetFloats()[f.Key] = f.Value;
                    }
                }
            }

            if (tprops.GetBooleans() != null)
            {
                foreach (var b in tprops.GetBooleans())
                {
                    if (!props.GetBooleans().ContainsKey(b.Key))
                    {
                        props.GetBooleans()[b.Key] = b.Value;
                    }
                }
            }

            if (tprops.GetColors() != null)
            {
                foreach (var c in tprops.GetColors())
                {
                    if (!props.GetColors().ContainsKey(c.Key))
                    {
                        props.GetColors()[c.Key] = c.Value;
                    }
                }
            }
        }

        var obj = new TMXParsedObject();

        obj.SetName(name);
        obj.SetType(type);
        obj.SetRect(rect);
        obj.SetRotation(rotation);
        obj.SetVisible(visible == 0 ? false : true);
        obj.SetShape(shape);
        obj.SetPoints(points);
        obj.SetProperties(props);
        obj.SetGid(tileId);

        return obj;
    }

    private static Vector2i CalculateInfiniteMapSizeAtNode(string filename, Vector2i chunkSize, XmlNode parentNode, string xpath)
    {
        var size = new Vector2i(0, 0);
        var minPoint = new Vector2i(int.MaxValue, int.MaxValue);
        var maxPoint = new Vector2i(int.MinValue, int.MinValue);

        var chunkNodeList = parentNode.SelectNodes(xpath);

        for (int i = 0; i < chunkNodeList.Count; i++)
        {
            var node = chunkNodeList.Item(i);

            int chunkX = 0;
            int chunkY = 0;

            if (node.Attributes["x"] == null || !int.TryParse(node.Attributes["x"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out chunkX))
            {
                Debug.LogError("TMX Importer: Can't parse chunk x offset in file " + filename);
                return size;
            }

            if (node.Attributes["y"] == null || !int.TryParse(node.Attributes["y"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out chunkY))
            {
                Debug.LogError("TMX Importer: Can't parse chunk y offset in file " + filename);
                return size;
            }

            if (chunkX < minPoint.x)
            {
                minPoint.x = chunkX;
            }

            if (chunkY < minPoint.y)
            {
                minPoint.y = chunkY;
            }

            if (chunkX > maxPoint.x)
            {
                maxPoint.x = chunkX;
            }

            if (chunkY > maxPoint.y)
            {
                maxPoint.y = chunkY;
            }
        }

        if (minPoint.x == int.MaxValue || maxPoint.x == int.MinValue)
        {
            size.x = 0;
        }
        else
        {
            size.x = (maxPoint.x + chunkSize.x) - minPoint.x;
        }

        if (minPoint.y == int.MaxValue || maxPoint.y == int.MinValue)
        {
            size.y = 0;
        }
        else
        {
            size.y = (maxPoint.y + chunkSize.y) - minPoint.y;
        }

        return size;
    }

    private static void ProcessTMX(string filename)
    {
        mPendingLayers = new List<PendingLayerWrite>();

        var writer = CreateAssetWriter();
        if (writer == null)
        {
            Debug.LogError("TMX Importer: Failed to create parsed TMX file for file " + filename);
        }

        var xmlDoc = OpenXML(filename);

        if (xmlDoc == null)
        {
            Debug.LogError("TMX Importer: Can't parse TMX file " + filename);
            return;
        }

        // Write header, magic number and version
        writer.Write(RetroBlitInternal.RBTilemapTMX.RetroBlit_TMX_MAGIC);
        writer.Write(RetroBlitInternal.RBTilemapTMX.RetroBlit_TMX_VERSION);
        writer.Write(RetroBlitInternal.RBTilemapTMX.RetroBlit_TMX_TYPE_MAP);

        var mapNodeElements = xmlDoc.GetElementsByTagName("map");
        var mapNode = mapNodeElements.Count > 0 ? mapNodeElements.Item(0) : null;
        if (mapNode == null)
        {
            Debug.LogError("TMX Importer: Parsing error, <map> not found in file " + filename);
            return;
        }

        if (mapNode.Attributes["orientation"] != null)
        {
            if (mapNode.Attributes["orientation"].Value.ToLowerInvariant() != "orthogonal")
            {
                Debug.LogError("TMX Importer: Only orthogonal maps supported, can't import " + filename);
                return;
            }
        }
        else
        {
            Debug.LogError("TMX Importer: Orientation attribute not found in file " + filename);
            return;
        }

        int tmxMapWidth = 0;
        int tmxMapHeight = 0;
        if (mapNode.Attributes["width"] != null && mapNode.Attributes["height"] != null)
        {
            if (!int.TryParse(mapNode.Attributes["width"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out tmxMapWidth) ||
                !int.TryParse(mapNode.Attributes["height"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out tmxMapHeight))
            {
                Debug.LogError("TMX Importer: Can't parse width, or height of map in file " + filename);
                return;
            }
        }
        else
        {
            Debug.LogError("TMX Importer: Width and/or height attributes not found in " + filename);
            return;
        }

        if (tmxMapWidth <= 0 || tmxMapHeight <= 0)
        {
            // Nothing to do
            return;
        }

        bool infinite = false;
        if (mapNode.Attributes["infinite"] != null)
        {
            int infiniteValue;

            if (!int.TryParse(mapNode.Attributes["infinite"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out infiniteValue))
            {
                Debug.LogError("TMX Importer: Can't parse infinite flag in file " + filename);
                return;
            }

            infinite = infiniteValue > 0 ? true : false;
        }

        Color32 backgroundColor = new Color32(0, 0, 0, 255);
        if (mapNode.Attributes["backgroundcolor"] != null)
        {
            string backgroundColorHTML = mapNode.Attributes["backgroundcolor"].Value.Trim();

            if ((backgroundColorHTML.Length != 9 && backgroundColorHTML.Length != 7) || backgroundColorHTML[0] != '#')
            {
                Debug.LogError("TMX Importer: Can't parse background color in file " + filename);
                return;
            }

            uint rgba;
            int r, g, b, a;

            if (!uint.TryParse(
                backgroundColorHTML.Substring(1),
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out rgba))
            {
                Debug.LogError("TMX Importer: Can't parse background color in file " + filename);
                return;
            }

            a = 255;

            if (backgroundColorHTML.Length == 9)
            {
                a = (int)((rgba & 0xFF000000) >> 24);
            }

            r = (int)((rgba & 0x00FF0000) >> 16);
            g = (int)((rgba & 0x0000FF00) >> 8);
            b = (int)((rgba & 0x000000FF) >> 0);

            backgroundColor = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
        }

        int chunkWidth = 16;
        int chunkHeight = 16;

        // Figure out chunk size ofr infinite maps
        if (infinite)
        {
            // Find chunk size by grabbing it from the first layer found
            var chunkNode = infinite ? mapNode.SelectSingleNode("//chunk") : null;
            if (chunkNode != null)
            {
                if (chunkNode.Attributes["width"] != null)
                {
                    if (!int.TryParse(chunkNode.Attributes["width"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out chunkWidth))
                    {
                        Debug.LogError("TMX Importer: Can't parse chunk width in file " + filename);
                        return;
                    }
                }

                if (chunkNode.Attributes["height"] != null)
                {
                    if (!int.TryParse(chunkNode.Attributes["height"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out chunkHeight))
                    {
                        Debug.LogError("TMX Importer: Can't parse chunk height in file " + filename);
                        return;
                    }
                }
            }
        }

        if (infinite)
        {
            var infiniteMapSize = CalculateInfiniteMapSizeAtNode(filename, new Vector2i(chunkWidth, chunkHeight), mapNode, "//chunk");
            tmxMapWidth = infiniteMapSize.width;
            tmxMapHeight = infiniteMapSize.height;
        }

        writer.Write(tmxMapWidth);
        writer.Write(tmxMapHeight);
        writer.Write(backgroundColor.r);
        writer.Write(backgroundColor.g);
        writer.Write(backgroundColor.b);
        writer.Write(backgroundColor.a);
        writer.Write(infinite);
        writer.Write(chunkWidth);
        writer.Write(chunkHeight);

        WritePropertiesIfAvailable(filename, mapNode, writer);

        var tmxInfo = new TMXDef();

        tmxInfo.chunkSize = new Vector2i(chunkWidth, chunkHeight);

        tmxInfo.relativePath = Path.GetDirectoryName(filename) + "/";
        tmxInfo.tmxFileName = filename;

        if (!LoadTMXInfo(filename, mapNode, ref tmxInfo))
        {
            Debug.LogError("TMX Importer: Could not load tileset info for file " + filename);
            return;
        }

        tmxInfo.infinite = infinite;

        string tmxFolder = Path.GetDirectoryName(filename) + "/" + Path.GetFileNameWithoutExtension(filename) + ".tmx.rb";
        if (Directory.Exists(tmxFolder))
        {
            // Try to wipe out all *.bytes files if folder exists
            System.IO.DirectoryInfo di = new DirectoryInfo(tmxFolder);

            string fileName = string.Empty;

            try
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    fileName = file.Name;
                    if (file.Extension == ".bytes" || (file.Extension == ".meta" && file.Name.Contains(".bytes")))
                    {
                        file.Delete();
                    }
                }
            }
            catch (System.Exception e)
            {
                if (fileName != null && fileName.Length > 0)
                {
                    Debug.LogError("TMX Importer: Failed to delete old generated TMX file " + fileName + ", another process may be using the file. " + e.ToString());
                }
                else
                {
                    Debug.LogError("TMX Importer: Failed to delete old generated TMX files for file " + filename + ", " + e.ToString());
                }

                return;
            }
        }

        try
        {
            Directory.CreateDirectory(tmxFolder);
        }
        catch (System.Exception e)
        {
            Debug.LogError("TMX Importer: Failed to create a folder for TMX file " + filename + ". " + e.ToString());
            return;
        }

        WriteTSXProperties(tmxInfo, writer);

        ProcessTMXLayerAndObjectGroups(mapNode, tmxInfo, writer, tmxFolder);

        writer.Write(RetroBlitInternal.RBTilemapTMX.RetroBlit_TMX_SECTION_END);

        MemoryStream memoryStream = (MemoryStream)writer.BaseStream;

        WriteAsset(tmxInfo, tmxFolder, "info.bytes", memoryStream);

        FlushPendingLayers(tmxFolder);
    }

    private static XmlNode GetFirstChildNode(XmlNode parentNode, string childName)
    {
        XmlNode childNode = null;
        for (int j = 0; j < parentNode.ChildNodes.Count; j++)
        {
            XmlNode node = parentNode.ChildNodes.Item(j);
            if (node.LocalName.ToLowerInvariant() == childName)
            {
                childNode = node;
                break;
            }
        }

        return childNode;
    }

    private static void WriteProperties(BinaryWriter writer, TMXPropertiesDef props)
    {
        var strings = props.GetStrings();
        if (strings != null)
        {
            writer.Write(strings.Keys.Count);
            foreach (var str in strings)
            {
                writer.Write(str.Key);
                writer.Write(str.Value);
            }
        }
        else
        {
            writer.Write((int)0);
        }

        var booleans = props.GetBooleans();
        if (booleans != null)
        {
            writer.Write(booleans.Keys.Count);
            foreach (var b in booleans)
            {
                writer.Write(b.Key);
                writer.Write(b.Value);
            }
        }
        else
        {
            writer.Write((int)0);
        }

        var ints = props.GetIntegers();
        if (ints != null)
        {
            writer.Write(ints.Keys.Count);
            foreach (var i in ints)
            {
                writer.Write(i.Key);
                writer.Write(i.Value);
            }
        }
        else
        {
            writer.Write((int)0);
        }

        var floats = props.GetFloats();
        if (floats != null)
        {
            writer.Write(floats.Keys.Count);
            foreach (var f in floats)
            {
                writer.Write(f.Key);
                writer.Write(f.Value);
            }
        }
        else
        {
            writer.Write((int)0);
        }

        var colors = props.GetColors();
        if (colors != null)
        {
            writer.Write(colors.Keys.Count);
            foreach (var c in colors)
            {
                writer.Write(c.Key);
                writer.Write(c.Value.r);
                writer.Write(c.Value.g);
                writer.Write(c.Value.b);
                writer.Write(c.Value.a);
            }
        }
        else
        {
            writer.Write((int)0);
        }
    }

    private static void WriteTSXProperties(TMXDef tmxInfo, BinaryWriter writer)
    {
        writer.Write(RetroBlitInternal.RBTilemapTMX.RetroBlit_TSX_PROPERTIES);

        int tsxIndex = 0;

        foreach (var tsx in tmxInfo.properties)
        {
            writer.Write(tsxIndex);
            foreach (var item in tsx)
            {
                writer.Write(item.Key);
                var props = item.Value;

                if (props == null)
                {
                    props = new TMXPropertiesDef();
                }

                WriteProperties(writer, props);
            }

            // TID -1 to signify no more properties for this tsx
            writer.Write(-1);

            tsxIndex++;
        }

        // TSX Index - 1 to signify that there are no more properties
        writer.Write(-1);
    }

    private static bool LoadTMXInfo(string tmxFileName, XmlNode mapNode, ref TMXDef tmxInfo)
    {
        int tileSetsCount = XMLCountChildren(mapNode.ChildNodes, "tileset");
        if (tileSetsCount == 0)
        {
            Debug.LogError("TMX Importer: Parsing error, could not find any tilesets for file " + tmxFileName);
            return false;
        }

        tmxInfo.firstGids.Capacity = tileSetsCount;
        for (int i = 0; i < mapNode.ChildNodes.Count; i++)
        {
            XmlNode node = mapNode.ChildNodes.Item(i);
            if (node.LocalName.ToLowerInvariant() == "tileset")
            {
                if (node.Attributes["firstgid"] == null)
                {
                    Debug.LogError("TMX Importer: Parsing error, tileset does not have firstgid attribute in file " + tmxFileName);
                    return false;
                }

                int firstGid = 0;
                if (!int.TryParse(node.Attributes["firstgid"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out firstGid))
                {
                    Debug.LogError("TMX Importer: Parsing error, tileset firstgid invalid in file " + tmxFileName);
                    return false;
                }

                tmxInfo.firstGids.Add(firstGid);

                var tilesetNode = node;
                var tsxSourceFileName = tmxFileName;

                // If tileset is not embedded then load it up now
                if (node.Attributes["source"] != null)
                {
                    string sourceFileName = node.Attributes["source"].Value.Trim();
                    sourceFileName = sourceFileName.Replace('\\', '/');

                    var tsxFileName = string.Empty;

                    if (Path.IsPathRooted(sourceFileName))
                    {
                        tsxFileName = sourceFileName;
                        Debug.Log("Rooted file: " + tsxFileName);
                    }
                    else
                    {
                        tsxFileName =
                            Path.GetDirectoryName(tmxFileName) + "/" +
                            sourceFileName;
                    }

                    tilesetNode = LoadTileSetNode(tsxFileName, sourceFileName);
                    tsxSourceFileName = tsxFileName;
                }

                if (tmxInfo.properties.Count >= 255)
                {
                    Debug.LogError("TMX Importer: RetroBlit can't support more than 255 tilesets in a TMX file " + tmxFileName);
                    return false;
                }

                var tsxTileProps = new Dictionary<int, TMXPropertiesDef>();
                tmxInfo.properties.Add(tsxTileProps);

                if (tilesetNode != null)
                {
                    LoadTileSetProperties(tsxSourceFileName, tsxTileProps, tilesetNode);
                }
            }
        }

        return true;
    }

    private static XmlNode LoadTileSetNode(string tsxFileName, string tsxFileNameOrig)
    {
        var xmlDoc = OpenXML(tsxFileName);

        if (xmlDoc == null)
        {
            Debug.LogError("TMX Importer: Can't parse TSX file " + tsxFileName);
            return null;
        }

        var tilesetNodeElements = xmlDoc.GetElementsByTagName("tileset");
        var tilesetNode = tilesetNodeElements.Count > 0 ? tilesetNodeElements.Item(0) : null;
        if (tilesetNode == null)
        {
            Debug.LogError("TMX Importer: TSX parsing error, <tileset> not found in " + tsxFileName);
            return null;
        }

        return tilesetNode;
    }

    private static bool LoadTileSetProperties(string filename, Dictionary<int, TMXPropertiesDef> tsxTileProps, XmlNode tilesetNode)
    {
        int tileNodesCount = XMLCountChildren(tilesetNode.ChildNodes, "tile");
        if (tileNodesCount <= 0)
        {
            return false;
        }

        for (int i = 0; i < tilesetNode.ChildNodes.Count; i++)
        {
            XmlNode tileNode = tilesetNode.ChildNodes.Item(i);
            if (tileNode.LocalName.ToLowerInvariant() == "tile")
            {
                if (tileNode.Attributes["id"] == null)
                {
                    Debug.LogError("TMX Parser: TSX parsing error, <tile> does not have id in file " + filename);
                    return false;
                }

                var tileId = -1;
                if (!int.TryParse(tileNode.Attributes["id"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out tileId) || tileId < 0)
                {
                    Debug.LogError("TMX Parser: TSX parsing error, <tile> id invalid in file " + filename);
                    return false;
                }

                var propsNode = GetFirstChildNode(tileNode, "properties");

                if (propsNode != null)
                {
                    var props = ReadProperties(filename, propsNode);
                    if (props != null)
                    {
                        tsxTileProps.Add(tileId, props);
                    }
                }
                else
                {
                    tsxTileProps.Add(tileId, null);
                }
            }
        }

        return true;
    }

    private static TMXPropertiesDef ReadProperties(string filename, XmlNode propsNode)
    {
        if (propsNode == null)
        {
            return null;
        }

        var properties = new TMXPropertiesDef();
        int propCount = 0;

        for (int j = 0; j < propsNode.ChildNodes.Count; j++)
        {
            XmlNode node = propsNode.ChildNodes.Item(j);
            if (node.LocalName.ToLowerInvariant() == "property")
            {
                if (node.Attributes["name"] != null && node.Attributes["value"] != null)
                {
                    var name = node.Attributes["name"].Value.Trim();
                    var value = node.Attributes["value"].Value;
                    var type = "string";
                    if (node.Attributes["type"] != null)
                    {
                        type = node.Attributes["type"].Value.Trim().ToLowerInvariant();
                    }

                    if (type == "bool")
                    {
                        if (value == "true")
                        {
                            properties.Add(name, true);
                        }
                        else
                        {
                            properties.Add(name, false);
                        }
                    }
                    else if (type == "color")
                    {
                        Color color;
                        if (ColorUtility.TryParseHtmlString(value, out color))
                        {
                            properties.Add(name, color);
                        }
                        else
                        {
                            Debug.LogError("TMX Importer: Parsing error, property value format invalid for color, should be #RRGGBBAA in file " + filename);
                        }
                    }
                    else if (type == "int")
                    {
                        int intVal = 0;
                        if (!int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out intVal))
                        {
                            Debug.LogError("TMX Importer: Parsing error, property value format invalid for integer in file " + filename);
                        }
                        else
                        {
                            properties.Add(name, intVal);
                        }
                    }
                    else if (type == "float")
                    {
                        float floatVal = 0;
                        if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out floatVal))
                        {
                            Debug.LogError("TMX Importer: Parsing error, property value format invalid for float in file " + filename);
                        }
                        else
                        {
                            properties.Add(name, floatVal);
                        }
                    }
                    else
                    {
                        // Everything else, includes strings, files, and unrecognized types
                        properties.Add(name, value);
                    }

                    propCount++;
                }
            }
        }

        return propCount > 0 ? properties : null;
    }

    private static int XMLCountChildren(XmlNodeList list, string childName)
    {
        if (list == null)
        {
            return 0;
        }

        childName = childName.ToLowerInvariant();

        int c = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (list.Item(i).LocalName.ToLowerInvariant() == childName)
            {
                c++;
            }
        }

        return c;
    }

#if !RETROBLIT_STANDALONE
    /// <summary>
    /// Class for registering Update callback
    /// </summary>
    [InitializeOnLoad]
    public class RetroBlitTMXPostProcessorStartup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        static RetroBlitTMXPostProcessorStartup()
        {
            EditorApplication.update += EditorUpdate;
        }
    }
#endif

    /// <summary>
    /// Exposes some extra access to TMXObject
    /// </summary>
    private class TMXParsedObject : TMXMapAsset.TMXObject
    {
        /// <summary>
        /// Set name
        /// </summary>
        /// <param name="name">name</param>
        public void SetName(string name)
        {
            mName = name;
        }

        /// <summary>
        /// Set type
        /// </summary>
        /// <param name="type">Type</param>
        public void SetType(string type)
        {
            mType = type;
        }

        /// <summary>
        /// Set shape
        /// </summary>
        /// <param name="shape">Shape</param>
        public void SetShape(Shape shape)
        {
            mShape = shape;
        }

        /// <summary>
        /// Set rectangular area
        /// </summary>
        /// <param name="rect">Rectangular area</param>
        public void SetRect(Rect2i rect)
        {
            mRect = rect;
        }

        /// <summary>
        /// Set rotation
        /// </summary>
        /// <param name="rotation">rotation</param>
        public void SetRotation(float rotation)
        {
            mRotation = rotation;
        }

        /// <summary>
        /// Set visibility flag
        /// </summary>
        /// <param name="visible">Visibility flag</param>
        public void SetVisible(bool visible)
        {
            mVisible = visible;
        }

        /// <summary>
        /// Set points
        /// </summary>
        /// <param name="points">Points</param>
        public void SetPoints(List<Vector2i> points)
        {
            mPoints = points;
        }

        /// <summary>
        /// Set custom properties
        /// </summary>
        /// <param name="props">Properties</param>
        public void SetProperties(TMXMapAsset.TMXProperties props)
        {
            mProperties = props;
        }

        public void SetGid(int gid)
        {
            mTileId = gid;
        }
    }

    /// <summary>
    /// TMX definition
    /// </summary>
    private class TMXDef
    {
        /// <summary>
        /// Chunk size
        /// </summary>
        public Vector2i chunkSize;

        /// <summary>
        /// Infinite flag
        /// </summary>
        public bool infinite;

        /// <summary>
        /// List of firstgids in TSX tilesets
        /// </summary>
        public List<int> firstGids = new List<int>();

        /// <summary>
        /// List of all tile properties
        /// </summary>
        public List<Dictionary<int, TMXPropertiesDef>> properties = new List<Dictionary<int, TMXPropertiesDef>>();

        /// <summary>
        /// Count of layers
        /// </summary>
        public int layerCount;

        /// <summary>
        /// Object groups
        /// </summary>
        public List<TMXMapAsset.TMXObjectGroup> objectGroups = new List<TMXMapAsset.TMXObjectGroup>();

        /// <summary>
        /// List of layers
        /// </summary>
        public List<int> layerList = new List<int>();

        /// <summary>
        /// All template objects
        /// </summary>
        public Dictionary<string, TMXParsedObject> templateObjects = new Dictionary<string, TMXParsedObject>();

        /// <summary>
        /// Relative path of the TMX file
        /// </summary>
        public string relativePath;

        /// <summary>
        /// Full TMX path
        /// </summary>
        public string tmxFileName;
    }

    /// <summary>
    /// Layer definition
    /// </summary>
    private class LayerDef
    {
        /// <summary>
        /// List of chunks in the layer
        /// </summary>
        public List<Vector2i> chunks = new List<Vector2i>();
    }

    /// <summary>
    /// Exposes some TMXProperties data
    /// </summary>
    private class TMXPropertiesDef : TMXMapAsset.TMXProperties
    {
        /// <summary>
        /// Gets all string values
        /// </summary>
        /// <returns>String values</returns>
        public Dictionary<string, string> GetStrings()
        {
            return mStrings;
        }

        /// <summary>
        /// Get all boolean values
        /// </summary>
        /// <returns>Boolean values</returns>
        public Dictionary<string, bool> GetBooleans()
        {
            return mBooleans;
        }

        /// <summary>
        /// Get all integer values
        /// </summary>
        /// <returns>Integer values</returns>
        public Dictionary<string, int> GetIntegers()
        {
            return mIntegers;
        }

        /// <summary>
        /// Get all float values
        /// </summary>
        /// <returns>Float values</returns>
        public Dictionary<string, float> GetFloats()
        {
            return mFloats;
        }

        /// <summary>
        /// Get all color values
        /// </summary>
        /// <returns>Color values</returns>
        public Dictionary<string, Color32> GetColors()
        {
            return mColors;
        }

        /// <summary>
        /// Return true if there are no properties at all
        /// </summary>
        /// <returns>True if empty</returns>
        public bool IsEmpty()
        {
            if ((mStrings == null || mStrings.Count == 0) &&
                (mBooleans == null || mBooleans.Count == 0) &&
                (mIntegers == null || mIntegers.Count == 0) &&
                (mFloats == null || mFloats.Count == 0) &&
                (mColors == null || mColors.Count == 0))
            {
                return true;
            }

            return false;
        }
    }

    private class Chunk
    {
        /// <summary>
        /// Offset of the chunk
        /// </summary>
        public Vector2i offset;

        /// <summary>
        /// Size of the chunk
        /// </summary>
        public Vector2i size;

        /// <summary>
        /// Memory stream for writing the chunk
        /// </summary>
        public MemoryStream stream;

        /// <summary>
        /// BinaryWriter for writing the chunk
        /// </summary>
        public BinaryWriter writer;

        /// <summary>
        /// Constructor
        /// </summary>
        public Chunk()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }
    }

    private class GroupSettings
    {
        /// <summary>
        /// Offset of the group in pixels
        /// </summary>
        public Vector2i offset = Vector2i.zero;

        /// <summary>
        /// Visibility flag of the group
        /// </summary>
        public bool visible = true;

        /// <summary>
        /// Opacity of the group
        /// </summary>
        public float opacity = 1.0f;
    }
}
