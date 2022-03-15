namespace PictureShare_.Helpers
{
    public class Images
    {
        IWebHostEnvironment _env;
        public Images(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> Upload(IFormFile file, string path)
        {
            string extension = Path.GetExtension(file.FileName);
            string root = _env.WebRootPath;
            string uploadPath = $"{root}{path}";
            string newFilename = $"{Guid.NewGuid()}{extension}".ToLower();
            string filePath = $"{uploadPath}{newFilename}";

            Directory.CreateDirectory(uploadPath);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"{path}{newFilename}";
        }

        public bool Delete(string path)
        {
            path = _env.WebRootPath + path;
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
