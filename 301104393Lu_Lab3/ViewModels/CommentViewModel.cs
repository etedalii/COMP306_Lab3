using _301104393Lu_Lab3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _301104393Lu_Lab3.ViewModels
{
    public class CommentViewModel
    {
        public List<Movie> Movies { get; set; }
        public Movie Movie { get; set; }

        public List<Comment> Comments { get; set; }
        public Comment Comment { get; set; }
    }
}
