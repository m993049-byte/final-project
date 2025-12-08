using System.ComponentModel.DataAnnotations.Schema;

namespace final_project.Models
{
    public class usersaccounts
    {
        public int Id { get; set; }

        [Column("username")]
        public string name { get; set; }

        [Column("userpass")]
        public string pass { get; set; }

        public string role { get; set; }
    }
}
