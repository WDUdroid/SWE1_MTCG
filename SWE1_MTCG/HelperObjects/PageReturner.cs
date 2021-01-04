using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG.HelperObjects
{
    class PageReturner
    {
        public string Name;
        public string Bio;
        public string Image;

        public PageReturner(string _name, string _bio, string _image)
        {
            Name = _name;
            Bio = _bio;
            Image = _image;
        }
    }
}
