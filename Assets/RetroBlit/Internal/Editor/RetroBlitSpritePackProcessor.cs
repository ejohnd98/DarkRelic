////#define DEBUG_FILL
////#define DEBUG_TRIM
////#define DEBUG_DUMP_STEPS
////#define DEBUG_PERFORMANCE
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Sprite packer, packs folders of sprites into a single sprite texture
/// </summary>
public class RetroBlitSpritePackProcessor : AssetPostprocessor
{
    private const int MIN_DIMENSION = 64; // Must be 64 due to packing buffer
    private const int MAX_DIMENSION = 8192;

    private static string mProjectRootPath;
    private static bool mAssetRefreshPending = false;

#if DEBUG_PERFORMANCE
    private static System.Diagnostics.Stopwatch mStopwatch = new System.Diagnostics.Stopwatch();
#endif

    private enum JobType
    {
        ReadFiles,
        TrimSprites,
        DrawSprites
    }

    private static void EditorUpdate()
    {
        if (mAssetRefreshPending)
        {
            AssetDatabase.Refresh();
            mAssetRefreshPending = false;
        }
    }

    /// <summary>
    /// Job entry thread
    /// </summary>
    /// <param name="param">Job parameters</param>
    private static void JobThread(object param)
    {
        var job = (Job)param;

        switch (job.type)
        {
            case JobType.ReadFiles:
                JobReadFiles(job);
                break;

            case JobType.TrimSprites:
                JobTrimSprites(job);
                break;

            case JobType.DrawSprites:
                JobDrawSprites(job);
                break;

            default:
                // Do nothing
                break;
        }
    }

    /// <summary>
    /// Read encoded image data
    /// </summary>
    /// <param name="job">Part of the job</param>
    private static void JobReadFiles(Job job)
    {
        for (int i = job.startIndex; i <= job.endIndex; i++)
        {
            var fullSpriteFile = Path.GetFullPath(job.spriteFiles[i].filename);
            var relativeSpriteFile = fullSpriteFile.Substring(job.spriteFiles[i].sourceFolder.Length + 1).Replace('\\', '/');
            relativeSpriteFile = relativeSpriteFile.Substring(0, relativeSpriteFile.LastIndexOf('.'));

#if RETROBLIT_STANDALONE
            job.sprites[i].pixels = Resources.LoadImageBytes(fullSpriteFile, out job.sprites[i].width, out job.sprites[i].height);
            job.sprites[i].size = job.sprites[i].width * job.sprites[i].height;
            job.sprites[i].pixelRect = new Rect2i(0, 0, job.sprites[i].width, job.sprites[i].height);
            job.sprites[i].name = relativeSpriteFile;
            job.sprites[i].imageBytes = null;
#else
            var file = File.OpenRead(fullSpriteFile);
            var imageBytes = new byte[file.Length];
            file.Read(imageBytes, 0, imageBytes.Length);

            file.Close();

            job.sprites[i].imageBytes = imageBytes;
            job.sprites[i].name = relativeSpriteFile;
#endif
        }
    }

    /// <summary>
    /// Trim sprites
    /// </summary>
    /// <param name="job">Part of the job</param>
    private static void JobTrimSprites(Job job)
    {
        for (int i = job.startIndex; i <= job.endIndex; i++)
        {
            if (job.sprites[i].pixels == null)
            {
                continue;
            }

            CalculateSpriteMargins(job.sprites[i]);
        }
    }

    /// <summary>
    /// Draw sprites into output texture
    /// </summary>
    /// <param name="job">Part of the job</param>
    private static void JobDrawSprites(Job job)
    {
        for (int i = job.startIndex; i <= job.endIndex; i++)
        {
            var sprite = job.sprites[i];

            if (sprite.pixels == null || sprite.placement.x < 0)
            {
                continue;
            }

            int startY = sprite.pixelRect.y;
            int endY = sprite.pixelRect.y + sprite.pixelRect.height;

            int startX = sprite.pixelRect.x;
            int endX = sprite.pixelRect.x + sprite.pixelRect.width;

            int desti = sprite.placement.x + (sprite.placement.y * job.outputWidth);
            int srci = sprite.pixelRect.x + (sprite.pixelRect.y * sprite.width);

            for (int y = startY; y < endY; y++)
            {
                Array.Copy(sprite.pixels, srci, job.outputBuf, desti, endX - startX);
                srci += sprite.width;
                desti += job.outputWidth;
            }

#if DEBUG_FILL
            for (int y = sprite.placement.y; y < sprite.placement.y + sprite.pixelRect.height; y++)
            {
                for (int x = sprite.placement.x; x < sprite.placement.x + sprite.pixelRect.width; x++)
                {
                    int j = x + y * job.outputWidth;
                    job.outputBuf[j] = Color.green;
                }
            }
#elif DEBUG_TRIM
            for (int y = sprite.placement.y; y < sprite.placement.y + sprite.pixelRect.height; y++)
            {
                for (int x = sprite.placement.x; x < sprite.placement.x + sprite.pixelRect.width; x++)
                {
                    if (y == sprite.placement.y || y == sprite.placement.y + sprite.pixelRect.height - 1 ||
                        x == sprite.placement.x || x == sprite.placement.x + sprite.pixelRect.width - 1)
                    {
                        int j = x + y * job.outputWidth;
                        job.outputBuf[j] = Color.red;
                    }
                }
            }
#endif
        }
    }

    private static bool AssetIsSpritePack(string asset)
    {
        if (Path.GetExtension(asset) == ".sp")
        {
            return true;
        }

        return false;
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        mProjectRootPath = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length) + "/";

        var newSPFiles = new List<string>();
        var deletedSPFiles = new List<string>();

        foreach (var asset in importedAssets)
        {
            if (asset.Contains("RetroBlit-ignore"))
            {
                continue;
            }

            if (AssetIsSpritePack(asset))
            {
                newSPFiles.Add(asset);
            }
        }

        foreach (var asset in movedAssets)
        {
            if (asset.Contains("RetroBlit-ignore"))
            {
                continue;
            }

            if (AssetIsSpritePack(asset))
            {
                newSPFiles.Add(asset);
            }
        }

        // Not too sure about this, maybe we should leave the generated assets even if the base .tmx file is deleted
        foreach (var asset in deletedAssets)
        {
            if (asset.Contains("RetroBlit-ignore"))
            {
                continue;
            }

            if (AssetIsSpritePack(asset))
            {
                deletedSPFiles.Add(asset);
            }
        }

        foreach (var asset in movedFromAssetPaths)
        {
            if (asset.Contains("RetroBlit-ignore"))
            {
                continue;
            }

            if (AssetIsSpritePack(asset))
            {
                deletedSPFiles.Add(asset);
            }
        }

        for (int i = 0; i < newSPFiles.Count; i++)
        {
            ProcessSPFile(newSPFiles[i]);
        }

        if (deletedSPFiles.Count > 0)
        {
            /* TODO: Delete spritepacks, might not be safe to do. */
        }

        if (deletedSPFiles.Count > 0 || newSPFiles.Count > 0)
        {
            mAssetRefreshPending = true;
        }
    }

    /// <summary>
    /// Process a sprite pack file and all the settings within
    /// </summary>
    /// <param name="spritePackFile">Sprite pack file</param>
    private static void ProcessSPFile(string spritePackFile)
    {
        List<string> sourceFolders = new List<string>();
        int outputWidth = -1;
        int outputHeight = -1;
        bool trimSprites = false;

        EditorUtility.ClearProgressBar();

        try
        {
            System.IO.StreamReader file = new System.IO.StreamReader(spritePackFile);
            string line;

            while ((line = file.ReadLine()) != null)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.Length == 0 || (trimmedLine[0] == '/' && trimmedLine[1] == '/'))
                {
                    continue;
                }

                var paramValue = trimmedLine.Split('=');
                if (paramValue.Length != 2)
                {
                    file.Close();
                    Debug.LogError("Sprite pack file " + spritePackFile + " has invalid line:\n" + line);
                    return;
                }

                paramValue[0] = paramValue[0].ToUpperInvariant();
                switch (paramValue[0])
                {
                    case "SOURCE_FOLDER":
                        sourceFolders.Add(paramValue[1].Replace('\\', '/'));
                        break;

                    case "OUTPUT_WIDTH":
                        if (!int.TryParse(paramValue[1], NumberStyles.Any, CultureInfo.InvariantCulture, out outputWidth))
                        {
                            file.Close();
                            Debug.LogError("Sprite pack file " + spritePackFile + " has invalid maxium width at line:\n" + line);
                            return;
                        }

                        break;

                    case "OUTPUT_HEIGHT":
                        if (!int.TryParse(paramValue[1], NumberStyles.Any, CultureInfo.InvariantCulture, out outputHeight))
                        {
                            file.Close();
                            Debug.LogError("Sprite pack file " + spritePackFile + " has invalid maxium height at line:\n" + line);
                            return;
                        }

                        break;

                    case "TRIM":
                        if (!bool.TryParse(paramValue[1], out trimSprites))
                        {
                            file.Close();
                            Debug.LogError("Sprite pack file " + spritePackFile + " has invalid trim boolean value at line:\n" + line);
                            return;
                        }

                        break;

                    default:
                        file.Close();
                        Debug.LogError("Sprite pack file " + spritePackFile + " has invalid parameter " + paramValue[0] + " at line:\n" + line);
                        return;
                }
            }

            file.Close();

            if (outputWidth == -1)
            {
                Debug.LogError("Output width is not specified in sprite pack file: " + spritePackFile + ". Use the OUTPUT_WIDTH parameter.");
                return;
            }

            if (outputHeight == -1)
            {
                Debug.LogError("Output height is not specified in sprite pack file: " + spritePackFile + ". Use the OUTPUT_HEIGHT parameter.");
                return;
            }

            if (sourceFolders.Count == 0)
            {
                Debug.LogError("Sprite source folders not specified in sprite pack file: " + spritePackFile + ". Use the SOURCE_FOLDER parameter.");
                return;
            }

            if (outputWidth < MIN_DIMENSION || outputWidth > MAX_DIMENSION ||
                outputHeight < MIN_DIMENSION || outputHeight > MAX_DIMENSION)
            {
                Debug.LogError("Output dimensions must be between " + MIN_DIMENSION + " and " + MAX_DIMENSION + " in file: " + spritePackFile);
                return;
            }

            file.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to read sprite pack file: " + spritePackFile + ", exception: " + e.ToString());
            EditorUtility.ClearProgressBar();
            return;
        }

        string spritePackFolder = Path.GetFullPath(spritePackFile);
        spritePackFolder = spritePackFolder.Substring(0, spritePackFolder.Length - 3) + ".sp.rb";

        bool ret = true;

        try
        {
            ret = PackSprites(sourceFolders, outputWidth, outputHeight, trimSprites, spritePackFolder);
        }
        catch
        {
            Debug.LogError("Failed to pack sprites!");
            EditorUtility.ClearProgressBar();
            return;
        }

        if (!ret)
        {
            return;
        }
    }

    /// <summary>
    /// Gather up all sprite file names from all source folders and their subfolders. This function is
    /// recursive
    /// </summary>
    /// <param name="files">Resulting list of files to append to</param>
    /// <param name="sourceFolder">Source folder</param>
    /// <param name="baseSourceFolder">Base source folder</param>
    private static void GetAllSpriteFiles(List<UnpackedSpriteFile> files, string sourceFolder, string baseSourceFolder)
    {
        try
        {
            var dirFiles = Directory.GetFiles(sourceFolder);
            for (int i = 0; i < dirFiles.Length; i++)
            {
                string file = dirFiles[i];
                int len = file.Length;
                if (len >= 5)
                {
                    if (file[len - 1] == 'g')
                    {
                        if (file[len - 2] == 'e' && file[len - 3] == 'p' && file[len - 4] == 'j')
                        {
                            files.Add(new UnpackedSpriteFile(file, baseSourceFolder));
                        }
                        else if (file[len - 2] == 'p' && file[len - 3] == 'j' && file[len - 4] == '.')
                        {
                            files.Add(new UnpackedSpriteFile(file, baseSourceFolder));
                        }
                        else if (file[len - 2] == 'n' && file[len - 3] == 'p' && file[len - 4] == '.')
                        {
                            files.Add(new UnpackedSpriteFile(file, baseSourceFolder));
                        }
                    }
                }
            }

            var dirs = Directory.GetDirectories(sourceFolder);
            for (int i = 0; i < dirs.Length; i++)
            {
                GetAllSpriteFiles(files, dirs[i], baseSourceFolder);
            }
        }
        catch
        {
            // Do nothing
        }
    }

    /// <summary>
    /// Get pixels from an encoded image buffer. This uses Unity APIs and must be ran on main thread
    /// </summary>
    /// <param name="sprite">Sprite to decode</param>
    /// <param name="workingTexture">Temporary working texture to use</param>
    /// <returns>True if successful</returns>
    private static bool GetPixelsFromImageBuffer(UnpackedSprite sprite, Texture2D workingTexture)
    {
#if !RETROBLIT_STANDALONE
        if (sprite.imageBytes == null)
        {
            return false;
        }

        workingTexture.LoadImage(sprite.imageBytes, false);

        sprite.pixels = workingTexture.GetPixels32();
        sprite.width = workingTexture.width;
        sprite.height = workingTexture.height;
        sprite.size = sprite.width * sprite.height;
        sprite.pixelRect = new Rect2i(0, 0, sprite.width, sprite.height);
#endif

        return true;
    }

    /// <summary>
    /// Calculates the margins of a sprite, essentially trimming away empty space (alpha = 0)
    /// </summary>
    /// <param name="sprite">Sprite to calculate margins for</param>
    private static void CalculateSpriteMargins(UnpackedSprite sprite)
    {
        var pixels = sprite.pixels;
        int left = sprite.width;
        int top = sprite.height;
        int right = 0;
        int bottom = 0;

        int i = 0;
        for (int y = 0; y < sprite.height; y++)
        {
            bool pixelPresent = false;

            for (int x = 0; x < sprite.width; x++)
            {
                if (pixels[i].a != 0)
                {
                    if (x < left)
                    {
                        left = x;
                    }

                    if (x > right)
                    {
                        right = x;
                    }

                    pixelPresent = true;
                }

                i++;
            }

            if (pixelPresent)
            {
                if (y < top)
                {
                    top = y;
                }

                if (y > bottom)
                {
                    bottom = y;
                }
            }
        }

        // If we didn't find any pixels then flag the sprite as empty
        if (left == sprite.width)
        {
            sprite.isEmpty = true;
            sprite.size = 0;
        }
        else
        {
            sprite.pixelRect = new Rect2i(left, top, right - left + 1, bottom - top + 1);
            sprite.size = sprite.pixelRect.width * sprite.pixelRect.height;
        }
    }

    /// <summary>
    /// Create worker threads and send them each a piece of a larger job to do
    /// </summary>
    /// <param name="jobs">List of jobs</param>
    private static void DoJobs(List<Job> jobs)
    {
        var threadList = new List<Thread>();
        for (int i = 0; i < jobs.Count; i++)
        {
            threadList.Add(new Thread(JobThread));
        }

        for (int i = 0; i < jobs.Count; i++)
        {
            threadList[i].Start(jobs[i]);
        }

        for (int i = 0; i < jobs.Count; i++)
        {
            threadList[i].Join();
        }
    }

    private static bool PackSprites(List<string> sourceFolders, int targetWidth, int targetHeight, bool trimSprites, string spritePackFolder)
    {
#if DEBUG_PERFORMANCE
        float startTime = Time.realtimeSinceStartup;

        mStopwatch.Reset();
        mStopwatch.Start();
#endif

        var jobs = new List<Job>();
        var spriteFiles = new List<UnpackedSpriteFile>();

        // Parts of the algorithm that can be multithreaded will use jobThreadCount amount of threads, based on the hardware capabilities
        var jobThreadCount = Environment.ProcessorCount <= 0 ? 1 : Environment.ProcessorCount;

        EditorUtility.DisplayProgressBar("Sprite packing", "Collect sprite file names", 0.0f);

        // Gather up all sprite file names from all source folders
        for (int i = 0; i < sourceFolders.Count; i++)
        {
            var fullSourceFolder = Path.GetFullPath(mProjectRootPath + sourceFolders[i]);

            GetAllSpriteFiles(spriteFiles, fullSourceFolder, fullSourceFolder);
        }

        // Sort to ensure consistent order between platforms, not strictly necessary but it's odd to see different sprite packs created on
        // different systems
        spriteFiles.Sort(CompareSpriteName);

        EditorUtility.DisplayProgressBar("Sprite packing", "Collect sprite file names", 0.05f);

#if DEBUG_PERFORMANCE
        mStopwatch.Stop();
        Debug.Log("PERF GetAllSpriteFiles: " + mStopwatch.ElapsedMilliseconds);

        mStopwatch.Reset();
        mStopwatch.Start();
#endif

        // No sprites to pack
        if (spriteFiles.Count == 0)
        {
            StringBuilder resourceFolders = new StringBuilder();
            for (int i = 0; i < sourceFolders.Count; i++)
            {
                resourceFolders.AppendLine(sourceFolders[i]);
            }

            var spritePackFile = spritePackFolder.Substring(0, spritePackFolder.Length - 3);
            Debug.LogError("No sprite files for Spritepack " + spritePackFile + " in the given source folders: \n" + resourceFolders.ToString());

            EditorUtility.ClearProgressBar();
            return true;
        }

        // Create empty sprites, one for each sprite file
        var sprites = new List<UnpackedSprite>(spriteFiles.Count);
        for (int i = 0; i < spriteFiles.Count; i++)
        {
            sprites.Add(new UnpackedSprite(i));
        }

        int chunkSize = spriteFiles.Count / jobThreadCount;
        if (chunkSize <= 0)
        {
            chunkSize = 1;
        }

#if DEBUG_PERFORMANCE
        mStopwatch.Reset();
        mStopwatch.Start();
#endif

        EditorUtility.DisplayProgressBar("Sprite packing", "Read sprite file raw data", 0.05f);

        // Read in all sprite image files into memory buffers. The data will still be encoded but
        // it will be in memory and ready to decode.
        jobs.Clear();
        int chunkOffset = 0;
        for (int i = 0; i < jobThreadCount; i++)
        {
            if (chunkOffset >= spriteFiles.Count)
            {
                break;
            }

            var job = new Job();
            job.type = JobType.ReadFiles;
            job.sprites = sprites;
            job.spriteFiles = spriteFiles;
            job.startIndex = chunkOffset;
            job.endIndex = chunkOffset + chunkSize;

            if (job.endIndex > spriteFiles.Count - 1)
            {
                job.endIndex = spriteFiles.Count - 1;
            }

            chunkOffset = job.endIndex + 1;

            jobs.Add(job);
        }

        DoJobs(jobs);

        EditorUtility.DisplayProgressBar("Sprite packing", "Read sprite file raw data", 0.15f);

#if DEBUG_PERFORMANCE
        mStopwatch.Stop();
        Debug.Log("PERF ReadFiles: " + mStopwatch.ElapsedMilliseconds);
#endif

#if DEBUG_PERFORMANCE
        mStopwatch.Reset();
        mStopwatch.Start();
#endif

#if !RETROBLIT_STANDALONE
        // Create a texture that will be reused for all sprite images being loaded
        var workingTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        workingTexture.anisoLevel = 1;
        workingTexture.filterMode = FilterMode.Point;

        EditorUtility.DisplayProgressBar("Sprite packing", "Decode sprite files", 0.15f);

        // Decode all sprite images and get their pixel data. We have to use Unity apis for that, a custom native decoder would require a dll, and a
        // C# decoder in safe mode would be too flow. Decoding images and getting the pixels must happen on main thread due to Unity limitations
        for (int i = 0; i < sprites.Count; i++)
        {
            if (!GetPixelsFromImageBuffer(sprites[i], workingTexture))
            {
                Debug.LogError("Failed to get pixels for sprite " + sprites[i].name);
                EditorUtility.ClearProgressBar();
                return false;
            }

            if (sprites[i].width > targetWidth || sprites[i].height > targetHeight)
            {
                Debug.LogError("The sprite " + sprites[i].name + " exceed maximum sprite pack width and/or height all on its own!");
                EditorUtility.ClearProgressBar();
                return false;
            }

            if (i % 32 == 0)
            {
                EditorUtility.DisplayProgressBar("Sprite packing", "Decode: " + sprites[i].name, ((i / (float)sprites.Count) * 0.20f) + 0.15f);
            }
        }

        EditorUtility.DisplayProgressBar("Sprite packing", "Decode sprite files", 0.35f);
#endif

#if DEBUG_PERFORMANCE
        mStopwatch.Stop();
        Debug.Log("PERF GetPixelsFromImageBuffer: " + mStopwatch.ElapsedMilliseconds);
#endif

        EditorUtility.DisplayProgressBar("Sprite packing", "Trim sprites", 0.35f);

        // If TRIM was set then trim empty (alpha=0) space from all sides of the sprites
        if (trimSprites)
        {
#if DEBUG_PERFORMANCE
            mStopwatch.Reset();
            mStopwatch.Start();
#endif

            jobs.Clear();
            chunkOffset = 0;
            for (int i = 0; i < jobThreadCount; i++)
            {
                if (chunkOffset >= sprites.Count)
                {
                    break;
                }

                var job = new Job();
                job.type = JobType.TrimSprites;
                job.sprites = sprites;
                job.spriteFiles = spriteFiles;
                job.startIndex = chunkOffset;
                job.endIndex = chunkOffset + chunkSize;

                if (job.endIndex > spriteFiles.Count - 1)
                {
                    job.endIndex = spriteFiles.Count - 1;
                }

                chunkOffset = job.endIndex + 1;

                jobs.Add(job);
            }

            DoJobs(jobs);

#if DEBUG_PERFORMANCE
            mStopwatch.Stop();
            Debug.Log("PERF TrimSprites: " + mStopwatch.ElapsedMilliseconds);
#endif
        }

        EditorUtility.DisplayProgressBar("Sprite packing", "Trim sprites", 0.40f);

#if DEBUG_PERFORMANCE
        mStopwatch.Reset();
        mStopwatch.Start();
#endif
        EditorUtility.DisplayProgressBar("Sprite packing", "Sort sprites", 0.40f);

        // Sort all sprites, biggest first, sorted by size (width * height)
        sprites.Sort(CompareSpriteSizeBiggestFirst);

        EditorUtility.DisplayProgressBar("Sprite packing", "Sort sprites", 0.45f);

#if DEBUG_PERFORMANCE
        mStopwatch.Stop();
        Debug.Log("PERF SorteSprites: " + mStopwatch.ElapsedMilliseconds);
#endif
        // Set placement of all sprites to invalid values
        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].placement = new Vector2i(-1, -1);
        }

#if DEBUG_PERFORMANCE
        mStopwatch.Reset();
        mStopwatch.Start();
#endif

        // Attempt to pack all sprites
        var spritesPacked = FitSpritesScanline(sprites, targetWidth, targetHeight);

#if DEBUG_PERFORMANCE
        mStopwatch.Stop();
        Debug.Log("PERF FitSpritesScanline: " + mStopwatch.ElapsedMilliseconds);
#endif

        EditorUtility.DisplayProgressBar("Sprite packing", "Packing", 0.80f);

        // Check for failure to pack all sprites
        if (spritesPacked != sprites.Count)
        {
            Debug.LogError("Could only fit " + (((float)spritesPacked / sprites.Count) * 100).ToString("#.##") + "% of sprites in " + spritePackFolder + " spritepack. Consider making the spritepack dimensions larger.");
            EditorUtility.ClearProgressBar();
            return false;
        }

        var outputBuf = new Color32[targetWidth * targetHeight];

#if DEBUG_PERFORMANCE
        mStopwatch.Reset();
        mStopwatch.Start();
#endif

        EditorUtility.DisplayProgressBar("Sprite packing", "Plotting", 0.80f);

        // Render all sprites into the final sprite pack texture
        jobs.Clear();
        chunkOffset = 0;
        for (int i = 0; i < jobThreadCount; i++)
        {
            if (chunkOffset >= sprites.Count)
            {
                break;
            }

            var job = new Job();
            job.type = JobType.DrawSprites;
            job.sprites = sprites;
            job.spriteFiles = spriteFiles;
            job.startIndex = chunkOffset;
            job.endIndex = chunkOffset + chunkSize;
            job.outputBuf = outputBuf;
            job.outputWidth = targetWidth;
            job.outputHeight = targetHeight;

            if (job.endIndex > spriteFiles.Count - 1)
            {
                job.endIndex = spriteFiles.Count - 1;
            }

            chunkOffset = job.endIndex + 1;

            jobs.Add(job);
        }

        DoJobs(jobs);

#if DEBUG_PERFORMANCE
        mStopwatch.Stop();
        Debug.Log("PERF DrawSprites: " + mStopwatch.ElapsedMilliseconds);
        mStopwatch.Reset();
        mStopwatch.Start();
#endif

        EditorUtility.DisplayProgressBar("Sprite packing", "Encoding", 0.90f);

        // Encode the final texture into a png file
        string outTextureFileName = Path.GetFullPath(spritePackFolder + "/spritepack.png");
        System.IO.FileInfo file = new System.IO.FileInfo(outTextureFileName);
        file.Directory.Create();

#if RETROBLIT_STANDALONE
        Resources.SaveImageBytes(outputBuf, targetWidth, targetHeight, file.FullName);
#else
        var outputTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
        outputTexture.SetPixels32(outputBuf);
        var pngBytes = outputTexture.EncodeToPNG();

        System.IO.File.WriteAllBytes(file.FullName, pngBytes);
#endif

#if DEBUG_PERFORMANCE
        mStopwatch.Stop();
        Debug.Log("PERF SaveTexture: " + mStopwatch.ElapsedMilliseconds);
        mStopwatch.Reset();
        mStopwatch.Start();
#endif

        EditorUtility.DisplayProgressBar("Sprite packing", "Creating lookup", 0.95f);

        // Create a lookup table file, which will contain all sprites and their coordinates
        CreateLookupFile(sprites, targetWidth, targetHeight, spritePackFolder);

#if DEBUG_PERFORMANCE
        mStopwatch.Stop();
        Debug.Log("PERF CreateLookupFile: " + mStopwatch.ElapsedMilliseconds);

        Debug.Log("PERF TOTAL: " + (Time.realtimeSinceStartup - startTime));
#endif

        EditorUtility.DisplayProgressBar("Sprite packing", "Done!", 1.0f);
        EditorUtility.ClearProgressBar();

        return true;
    }

    private static int CompareSpriteSizeBiggestFirst(UnpackedSprite sprite1, UnpackedSprite sprite2)
    {
        int diff = sprite2.size - sprite1.size;

        // Break ties by sequence number. This helps ensures the same sort order on all platforms
        if (diff == 0)
        {
            return sprite1.seq - sprite2.seq;
        }

        return diff;
    }

    private static int CompareSpriteName(UnpackedSpriteFile sprite1, UnpackedSpriteFile sprite2)
    {
        return sprite1.filename.CompareTo(sprite2.filename);
    }

    private static int FitSpritesScanline(List<UnpackedSprite> sprites, int sheetWidth, int sheetHeight)
    {
        int yOffset = 0;
        int i;

        var packingBuffer = new ushort[sheetWidth * sheetHeight];

        // Initialize packing buffer to all empty spaces
        for (int y = 0; y < sheetHeight; y++)
        {
            ushort fillCount = 0;
            for (int x = sheetWidth - 1; x >= 0; x--)
            {
                i = (y * sheetWidth) + x;
                packingBuffer[i] = ++fillCount;
            }
        }

        // Loop through all unpacked sprites, try to fit them in available empty space
        for (i = 0; i < sprites.Count; i++)
        {
            if (i % 32 == 0)
            {
                EditorUtility.DisplayProgressBar("Sprite packing", "Pack: " + sprites[i].name, ((i / (float)sprites.Count) * 0.35f) + 0.45f);
            }

            // If the sprite is empty then skip it
            if (sprites[i].isEmpty)
            {
                continue;
            }

            var spriteRect = sprites[i].pixelRect;

            int xEnd = sheetWidth - spriteRect.width;
            int yEnd = sheetHeight - spriteRect.height;

            bool found = false;

            // Scan up looking for available space
            for (int y = 0; y <= yEnd && !found; y++)
            {
                yOffset = y * sheetWidth;

                // Scan left to right
                for (int x = 0; x <= xEnd; x++)
                {
                    // Check packing gap length and empty flag at this location
                    int packLen = packingBuffer[yOffset + x];
                    bool empty = (packLen & 0x8000) > 0 ? false : true;
                    packLen = packLen & 0x7FFF;

                    // If not empty then skip ahead by length
                    if (!empty)
                    {
                        x += packLen - 1;
                        continue;
                    }

                    // If empty but not wide enough then skip ahead as well
                    if (packLen < spriteRect.width)
                    {
                        x += packLen - 1;
                        continue;
                    }

                    // Found a stretch long enough to fit first line of the sprite, look up now to see if rest of sprite would fit
                    found = true;
                    for (int y1 = 1; y1 < spriteRect.height; y1++)
                    {
                        int yOffset1 = (y + y1) * sheetWidth;

                        // Again get the packing gap length and empty flag at this location
                        int packLen1 = packingBuffer[yOffset1 + x];
                        bool empty1 = (packLen1 & 0x8000) > 0 ? false : true;
                        packLen1 = packLen1 & 0x7FFF;

                        // If not empty then sprite can't fit here
                        if (!empty1)
                        {
                            found = false;
                            break;
                        }

                        // If empty but not wide enough then sprite can't fit here either
                        if (packLen1 < spriteRect.width)
                        {
                            found = false;
                            break;
                        }
                    }

                    // Sprite fit! Update pack gaps
                    if (found)
                    {
                        for (int y1 = 0; y1 < spriteRect.height; y1++)
                        {
                            yOffset = (y + y1) * sheetWidth;

                            // Figure out if pixel to the right of the sprite is already filled, if so then we'll have to
                            // continue is length count, if not then we can start with 0
                            bool emptyOnRight = true;
                            ushort newPackLen = 0;
                            if (x + spriteRect.width < sheetWidth)
                            {
                                ushort rightPackLen = (ushort)packingBuffer[yOffset + x + spriteRect.width];
                                emptyOnRight = (rightPackLen & 0x8000) > 0 ? false : true;
                                rightPackLen = (ushort)(rightPackLen & 0x7FFF);

                                if (!emptyOnRight)
                                {
                                    newPackLen = rightPackLen;
                                }
                            }

                            int yOffsetX = yOffset + x;

                            // Fill the pixels taken by the sprite
                            int x1 = 0;
                            for (x1 = spriteRect.width - 1; x1 >= 0; x1--)
                            {
                                packingBuffer[yOffsetX + x1] = (ushort)(0x8000 | (++newPackLen));
                            }

                            // Fill the pixels to the left of the sprite up to the next packing segment
                            if (x1 + x >= 0)
                            {
                                var startingFlag = packingBuffer[yOffsetX + x1] & 0x8000;
                                if (startingFlag == 0)
                                {
                                    newPackLen = 0;
                                }

                                while (x1 + x >= 0 && (packingBuffer[yOffsetX + x1] & 0x8000) == startingFlag)
                                {
                                    packingBuffer[yOffsetX + x1] = (ushort)(startingFlag | (++newPackLen));
                                    x1--;
                                }
                            }
                        }

                        // Set the sprite placement
                        sprites[i].placement = new Vector2i(x, y);

#if DEBUG_DUMP_STEPS
                        DumpState(packingBuffer, sprites, sheetWidth, sheetHeight, i);
#endif

                        break;
                    }
                }
            }

            // Failed to fit sprite, return the amount of sprites packed so far
            if (!found)
            {
                return i;
            }
        }

        // Return the total sprites packed, which at this point should be sprites.Count
        return i;
    }

    /// <summary>
    /// Dumps the current packing buffer and currently packed sprites. For debugging purposes only.
    /// </summary>
    /// <param name="packingBuffer">Packing buffer to dump</param>
    /// <param name="sprites">Sprites to dump</param>
    /// <param name="width">Texture pack width</param>
    /// <param name="height">Texture pack height</param>
    /// <param name="step">Dump step, used to number dump files</param>
    private static void DumpState(ushort[] packingBuffer, List<UnpackedSprite> sprites, int width, int height, int step)
    {
        Color32[] pixels = new Color32[width * 2 * height];
        int i, j;

        Color32 background = new Color32(32, 32, 32, 255);

        for (i = 0; i < pixels.Length; i++)
        {
            pixels[i] = background;
        }

        int notPlacedCount = 0;
        for (i = 0; i < sprites.Count; i++)
        {
            if (sprites[i].pixels == null || sprites[i].placement.x < 0)
            {
                notPlacedCount++;
            }
        }

        for (i = 0; i < sprites.Count; i++)
        {
            var sprite = sprites[i];

            if (sprite.pixels == null || sprite.placement.x < 0)
            {
                continue;
            }

            int startY = sprite.pixelRect.y;
            int endY = sprite.pixelRect.y + sprite.pixelRect.height;

            int desti = sprite.placement.x + (sprite.placement.y * width * 2);
            int srci = sprite.pixelRect.x + (sprite.pixelRect.y * sprite.width);

            for (int y = startY; y < endY; y++)
            {
                for (int x = 0; x < sprite.pixelRect.width; x++)
                {
                    pixels[desti + x] = sprite.pixels[srci + x];
                    if (pixels[desti + x].a == 0)
                    {
                        pixels[desti + x] = background;
                    }
                }

                srci += sprite.width;
                desti += width * 2;
            }

#if DEBUG_FILL
            for (int y = sprite.placement.y; y < sprite.placement.y + sprite.pixelRect.height; y++)
            {
                for (int x = sprite.placement.x; x < sprite.placement.x + sprite.pixelRect.width; x++)
                {
                    j = x + y * width * 2;
                    pixels[j] = Color.green;
                }
            }
#elif DEBUG_TRIM
            if (notPlacedCount > 0)
            {
                for (int y = sprite.placement.y; y < sprite.placement.y + sprite.pixelRect.height; y++)
                {
                    for (int x = sprite.placement.x; x < sprite.placement.x + sprite.pixelRect.width; x++)
                    {
                        if (y == sprite.placement.y || y == sprite.placement.y + sprite.pixelRect.height - 1 ||
                            x == sprite.placement.x || x == sprite.placement.x + sprite.pixelRect.width - 1)
                        {
                            j = x + y * width * 2;
                            pixels[j] = Color.red;
                        }
                    }
                }
            }
#endif
        }

        j = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var len = packingBuffer[j++];
                var empty = (len & 0x8000) == 0;
                len = (ushort)(len & 0x7FFF);

                float scale = len / (float)width;

                byte lenColor = (byte)(((255 - 96) * scale) + 96);

                i = (y * width * 2) + x + width;

                if (empty)
                {
                    pixels[i] = new Color32(0, lenColor, 48, 255);
                }
                else
                {
                    pixels[i] = new Color32(lenColor, 48, 48, 255);
                }
            }
        }

        string outTextureFileName = Path.GetFullPath("c:/temp/packing_buffer_" + step.ToString("00000") + ".png");
        System.IO.FileInfo file = new System.IO.FileInfo(outTextureFileName);
        file.Directory.Create();

#if RETROBLIT_STANDALONE
        Resources.SaveImageBytes(pixels, width * 2, height, file.FullName);
#else
        var outputTexture = new Texture2D(width * 2, height, TextureFormat.RGBA32, false);
        outputTexture.SetPixels32(pixels);
        var pngBytes = outputTexture.EncodeToPNG();

        System.IO.File.WriteAllBytes(file.FullName, pngBytes);
#endif
    }

    /// <summary>
    /// Creates the lookup file for sprite names and their coordinates
    /// </summary>
    /// <param name="sprites">Sprites</param>
    /// <param name="width">Output width</param>
    /// <param name="height">Output height</param>
    /// <param name="spritePackFolder">Output folder</param>
    private static void CreateLookupFile(List<UnpackedSprite> sprites, int width, int height, string spritePackFolder)
    {
        string outLookupFilename = Path.GetFullPath(spritePackFolder + "/info.bytes");

        /*
        Format:
#### - Count of sprites
        AAAA BB BB BB BB CC CC
        A - Sprite name hash
        B - Sprite rect (cast to unsigned shorts), possibly trimmed
        C - Sprite drawing offset x/y (unsigned shorts) (if trimmed it may be non-zero)
        */

        int validSpriteCount = 0;
        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i].placement.x >= 0)
            {
                validSpriteCount++;
            }
        }

        HashSet<int> allHashes = new HashSet<int>();

        var memoryStream = new MemoryStream();
        var writer = new BinaryWriter(memoryStream);

        writer.Write(RetroBlitInternal.RBRenderer.RetroBlit_SP_MAGIC);
        writer.Write(RetroBlitInternal.RBRenderer.RetroBlit_SP_VERSION);

        var spriteCountPos = writer.Seek(0, SeekOrigin.Current);
        writer.Write(validSpriteCount);

        int newSpriteCount = validSpriteCount;

        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i].placement.x >= 0)
            {
                var hash = RetroBlitInternal.RBUtil.StableStringHash(sprites[i].name);
                if (allHashes.Contains(hash))
                {
                    Debug.LogWarning("Found duplicate sprite \"" + sprites[i].name + "\" in sprite pack, dropping duplicate");
                    newSpriteCount--;
                    continue;
                }

                allHashes.Add(hash);

                writer.Write(hash);

                writer.Write((ushort)sprites[i].width);
                writer.Write((ushort)sprites[i].height);

                writer.Write((ushort)sprites[i].placement.x);
                writer.Write((ushort)(height - sprites[i].placement.y - sprites[i].pixelRect.height));
                writer.Write((ushort)sprites[i].pixelRect.width);
                writer.Write((ushort)sprites[i].pixelRect.height);

                writer.Write((ushort)sprites[i].pixelRect.x);
                writer.Write((ushort)(sprites[i].height - sprites[i].pixelRect.y - sprites[i].pixelRect.height));
            }
        }

        // Update sprite count if we dropped any
        if (newSpriteCount != validSpriteCount)
        {
            writer.Seek((int)spriteCountPos, SeekOrigin.Begin);
            writer.Write(newSpriteCount);
        }

        var fileStream = new FileStream(outLookupFilename, FileMode.Create, FileAccess.Write, FileShare.None, 65536);
        if (fileStream == null)
        {
            Debug.LogError("TMX Importer: Can't create sprite pack file " + outLookupFilename);
            return;
        }

        var data = memoryStream.ToArray();

        fileStream.Write(data, 0, data.Length);
        fileStream.Flush();
        fileStream.Close();
    }

    private struct Job
    {
        /// <summary>
        /// Type of job
        /// </summary>
        public JobType type;

        /// <summary>
        /// List of sprite files, this may or may not be used based on job type
        /// </summary>
        public List<UnpackedSpriteFile> spriteFiles;

        /// <summary>
        /// List of unpacked sprites
        /// </summary>
        public List<UnpackedSprite> sprites;

        /// <summary>
        /// Output color buffer, this may or may not be used based on job type
        /// </summary>
        public Color32[] outputBuf;

        /// <summary>
        /// Start index into list of sprites
        /// </summary>
        public int startIndex;

        /// <summary>
        /// End index into list of sprites
        /// </summary>
        public int endIndex;

        /// <summary>
        /// Sprite pack output texture width, this may or may not be used based on job type
        /// </summary>
        public int outputWidth;

        /// <summary>
        /// Sprite pack output texture height, this may or may not be used based on job type
        /// </summary>
        public int outputHeight;
    }

    private struct UnpackedSpriteFile
    {
        public string filename;
        public string sourceFolder;

        public UnpackedSpriteFile(string filename, string sourceFolder)
        {
            this.filename = filename;
            this.sourceFolder = sourceFolder;
        }
    }

    private class UnpackedSprite
    {
        public int seq;

        public string name;
        public byte[] imageBytes;

        public int width;
        public int height;
        public Color32[] pixels;
        public Rect2i pixelRect;
        public bool isEmpty;
        public int size;

        public Vector2i placement;

        public UnpackedSprite(int seq)
        {
            this.seq = seq;
        }
    }

#if !RETROBLIT_STANDALONE
    /// <summary>
    /// Class for registering Editor Update callback
    /// </summary>
    [InitializeOnLoad]
    private class RetroBlitSpritePackProcessorStartup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        static RetroBlitSpritePackProcessorStartup()
        {
            EditorApplication.update += EditorUpdate;
        }
    }
#endif
}
