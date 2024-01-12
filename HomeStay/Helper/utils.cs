namespace HomeStay.Helper
{
    public static class utils
    {
        public static async Task<string> UploadImage(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string? name )
        {
            try
            {
                if (name == null)
                    name = file.FileName;

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", sDirectory, name);

                var typeAccept = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);

                if (!typeAccept.Contains(fileExt.ToLower()))
                {
                    return null;
                }
                else
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return name;
                }
            }
            catch
            {
                return null;
            }

        }


        public static string AppendSeparator(string url)
        {
            return url.Contains("?") ? "&" : "?";
        }
    }
} 
