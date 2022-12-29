using System.Drawing;

namespace Phlog.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        // takes in the ImageFile that was passed
        // from the controller and returns a unique filename
        public string CreateUniqueFileName(IFormFile ImageFile) 
        {
            return Guid.NewGuid().ToString() + "-" + ImageFile.FileName;
        }

        // takes in the ImageFile that was passed
        // from the controller and upload it to the
        // server using the unique filename.
        public async void UploadImageFile(IFormFile ImageFile, string imageFileName) 
        {
            string filePath = webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar +
                              "uploads" + Path.DirectorySeparatorChar + imageFileName;

            Stream fileStream = new FileStream(filePath, FileMode.Create);
            await ImageFile.CopyToAsync(fileStream);
            fileStream.Close();
        }
    }
}
