using FluentValidation;
using ImageAPI.Contracts;

namespace ImageAPI.Validators
{
    public class ImageUploadRequestValidator : AbstractValidator<ImageUploadRequest>
    {
        const long MAX_FILE_SIZE = 2 * 1024 * 1024;
        const int DESCRIPTION_MAXIMUM_LENGTH = 300;

        public ImageUploadRequestValidator()
        {
            RuleFor(x => x.FileName).NotEmpty().WithMessage("FileName is required.");

            RuleFor(x => x.Description)
                .MaximumLength(DESCRIPTION_MAXIMUM_LENGTH).WithMessage("Description must be less than 300 characters.");

            RuleFor(x => x.Image).NotNull().WithMessage("Image is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Image).Must(file => file.Length <= MAX_FILE_SIZE).WithMessage("Image size must not exceed 2 MB.");
                });

            RuleFor(x => x.AllowImageOverwrite).NotNull();
        }
    }
}
