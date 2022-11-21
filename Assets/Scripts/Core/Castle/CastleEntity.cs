using System.Collections.Generic;
using System.Linq;

namespace Core.Castle
{
    public class CastleEntity
    {
        public Tower Tower { get; private set; }
        public Wall Wall { get; private set; }
        public List<Resource> Resources { get; private set; }

        public CastleEntity()
        {
            Tower = new Tower();
            Wall = new Wall();
            Resources = new List<Resource>()
            {
                new Resource("Resource_1"),
                new Resource("Resource_2"),
                new Resource("Resource_3")
            };
        }

        public Resource GetResource(string name)
        {
            return Resources.FirstOrDefault(r => r.Name == name);
        }
    }
}