// using System;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using HomeTrack.Domain.Enum; 

// namespace HomeTrack.Api.Models.Entities 
// {
//     [Table("confirmation_tokens")] 
//     public class ConfirmationToken
//     {
//         [Key]
//         public int Id { get; set; }

//         [Required]
//         public int UserId { get; set; } // Khóa ngoại

//         [Required]
//         [StringLength(256)] 
//         public required string Token { get; set; }

//         [Required]
//         public ConfirmationTokenType Type { get; set; } 

//         [Required]
//         public DateTime ExpirationAt { get; set; } 

//         [Required]
//         public DateTime CreatedAt { get; set; } 

//         public bool Used { get; set; }

//         // Navigation property đến User
//         [ForeignKey("UserId")]
//         public virtual User User { get; set; } = null!;

//         public ConfirmationToken()
//         {
//              // Khởi tạo giá trị mặc định nếu cần, hoặc sử dụng required.
//              // User property sẽ được EF Core load.
//         }
//     }
// }