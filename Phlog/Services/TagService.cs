using Phlog.Models;

namespace Phlog.Services
{
    public class TagService
    {
        // split the string of tag values into list of individual tags
        // based on empty spaces between each word
        public List<string> SplitTags(string tags)
        {
            List<string> result = tags.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            return result;
        }

    }
}
