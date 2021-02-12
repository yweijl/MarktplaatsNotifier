using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
    public class NewQueryDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public long UserId { get; set; }
    }
}