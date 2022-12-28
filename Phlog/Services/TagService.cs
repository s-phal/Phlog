using Phlog.Models;

namespace Phlog.Services
{
    public class TagService
    {
        public List<string> SplitTags(string tags)
        {
            //List<string> result = tags.Split(' ').ToList();
            List<string> result = tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return result;
        }

    }
}
