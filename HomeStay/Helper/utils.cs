using Newtonsoft.Json;

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

        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key , JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
           var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }


        public static bool IsValidEmail(string email)
        {
            if(email.Trim().EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;


            }catch
            {
                return false;
            }
        }

    }
} 
