using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;

namespace ChristmassyApp
{
    class Program
    {
        private static async Task<AnalysisResult> UploadAndAnalyzeImage(string imageFilePath)
        {
            VisionServiceClient VisionServiceClient = new VisionServiceClient("KEY GOES HERE", "HOSTNAME GOES HERE");
            Console.WriteLine("VisualServiceClient has been created");

            using (Stream imageFileStream = File.OpenRead(imageFilePath))
            {
                Console.WriteLine("Calling VisionServiceClient.AnalyzeImageAsync()...");
                VisualFeature[] visualFeatures = new VisualFeature[] { VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color,
                VisualFeature.Description, VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags };

                Console.WriteLine("Now awaiying results...");
                AnalysisResult analysisResult = await VisionServiceClient.AnalyzeImageAsync(imageFileStream, visualFeatures);
                return analysisResult;
            }
        }
        public static String[] GetFilesFrom(String searchFolder, String[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
            }
            return filesFound.ToArray();
        }

        static void Main(string[] args)
        {
            Task.Run(async () => {
                String searchFolder = @".";
                var filters = new String[] { "jpg", "jpeg", "png" };
                var imagesToAnalyse = GetFilesFrom(searchFolder, filters, false);
                foreach (var image in imagesToAnalyse)
                {
                    Console.WriteLine(image);
                }
                foreach (var image in imagesToAnalyse)
                {
                    Console.WriteLine("NOW ANALYSISNG: " + image);
                    AnalysisResult myResult = await UploadAndAnalyzeImage(image);

                    string destinationPath = "";

                    if (myResult.Tags.Length != 0)
                    {
                        Boolean found = false;

                        if (myResult.Tags[0].Name.Equals("hot") && myResult.Tags[1].Name.Equals("dog"))
                        {
                            destinationPath = "Hotdog" + image.Substring(1);
                            found = true;
                        }

                        if (found.Equals(false))
                        {
                            System.IO.Directory.CreateDirectory("Not Hotdog");
                            destinationPath = "Not Hotdog" + image.Substring(1);
                        }
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory("Not Hotdog");
                        destinationPath = "Not Hotdog" + image.Substring(1);
                    }

                    System.IO.File.Move(image, destinationPath);
                }
            }).GetAwaiter().GetResult();


        }
    }
}
