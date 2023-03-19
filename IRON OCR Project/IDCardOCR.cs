using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronOcr;
using IronSoftware.Drawing;

namespace IRON_OCR_Project
{
    public class IDCardOCR
    {
        private int x_axis;
        private int y_axis;
        private int width;
        private int height;
        private string imagePath;
        private int scale;
        private string saveFileWithName;
        IronTesseract Ocr;
        public IDCardOCR(string imagePath, int scale, string saveFileWithName,int xaxis , int yaxis ,int width , int height)
        {
            this.x_axis = xaxis;
            this.y_axis = yaxis; 
            this.width = width;
            this.height = height;
            this.imagePath = imagePath;
            this.scale = scale;
            this.saveFileWithName = saveFileWithName;
            this.Ocr = new IronTesseract();
        }

        private CropRectangle CropRect { 
            get{
                return new IronSoftware.Drawing.CropRectangle(x_axis, y_axis, width, height);
            }
        }

        private void settingUp_OCR()
        {
            this.Ocr.Language = OcrLanguage.ArabicAlphabetBest;
            this.Ocr.AddSecondaryLanguage(OcrLanguage.ArabicAlphabetBest);
            this.Ocr.UseCustomTesseractLanguageFile(@"tessdata_arabic-master\ara-Amiri.traineddata");
            this.Ocr.UseCustomTesseractLanguageFile(@"tessdata_arabic-master\ara-Amiri-layer.traineddata");
            this.Ocr.Configuration.BlackListCharacters = "1234567890\"'؟!÷×؛~ْ^%،<>+=*::!@#$%^&*()_+QWERTYUIOPASDFGHJKL;ZXCVBNM,??qwertyuiopasdfghjklzxcvbnm~`$#^*_}{][|\\@¢©«»°±·×‑–—‘’“”•…′″€™←↑→↓↔⇄⇒∅∼≅≈≠≤≥≪≫⌁⌘○◔◑◕●☐☑☒☕☮☯☺♡⚓✓✰";
            this.Ocr.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.Auto;
            this.Ocr.Configuration.ReadBarCodes = false;

        }

        public void print_ID_Info( Predicate<bool> useFilter)
        {
            settingUp_OCR();

            using (var ocrInput = new OcrInput())
            {
                CropRectangle rec = this.CropRect;

                Bitmap bm = (Bitmap)Bitmap.FromFile(imagePath);

                ocrInput.AddImage(bm, rec);

                if (useFilter(false))
                {

                }

                //ocrInput.Erode();
                ocrInput.Binarize();
                ocrInput.DeNoise();
                ocrInput.SelectTextColor(IronSoftware.Drawing.Color.Black);
                
                var newImage = ocrInput.StampCropRectangleAndSaveAs(rec,IronSoftware.Drawing.Color.Red, saveFileWithName);
                ocrInput.Scale(scale);

                var ocrResult = Ocr.Read(ocrInput);

                Console.WriteLine(ocrResult.Text);
                Console.WriteLine("-----------------------------------------------------------------------");
                ocrResult.SaveAsSearchablePdf($"../../../OCR_Result/{saveFileWithName}.pdf");
                ocrResult.SaveAsTextFile($"../../../OCR_Result/{saveFileWithName}.txt");

            }

        }
    }
}
