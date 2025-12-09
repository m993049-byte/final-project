using System.ComponentModel.DataAnnotations.Schema;

namespace final_project.Models
{
    public class usersaccounts
    {
        public int Id { get; set; }

        public string name { get; set; }


        public string pass { get; set; }

        public string role { get; set; }
    }
}
