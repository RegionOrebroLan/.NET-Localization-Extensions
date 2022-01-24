using System.ComponentModel.DataAnnotations;

namespace Application.Models.Forms
{
	public class Form
	{
		#region Properties

		[Display(Name = "Age")]
		[Range(0, 200, ErrorMessage = "\"{0}\" must be at least {1} and at most {2}.")]
		[Required(ErrorMessage = "\"{0}\" is required.")]
		public virtual int? Age { get; set; }

		[Display(Name = "Name")]
		[Required(ErrorMessage = "\"{0}\" is required.")]
		[StringLength(10, ErrorMessage = "\"{0}\" must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
		public virtual string Name { get; set; }

		#endregion
	}
}