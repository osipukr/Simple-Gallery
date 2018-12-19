using Microsoft.AspNet.Identity;
using Simply_Gallery.DAL.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Simply_Gallery.DAL.Identity
{
    // Кастомная валидация пользователей
    public class ApplicationUserValidator : UserValidator<ApplicationUser>
    {
        public ApplicationUserValidator(ApplicationUserManager manager)
            : base(manager)
        {
            // Юзернейм должен содержать только алфавитно-цифровые символы
            AllowOnlyAlphanumericUserNames = true;

            // Email пользователя должен быть уникальным
            RequireUniqueEmail = true;
        }

        public override async Task<IdentityResult> ValidateAsync(ApplicationUser user)
        {
            var nameBanned = new string[]
            {
                "admin", "админ", "user", "пользователь"
            };

            var emailBannder = new string[]
            {
                "@spam.com", "@temp.ru"
            };

            var result = await base.ValidateAsync(user);

            if (nameBanned.FirstOrDefault(x => user.UserName.ToLower().Contains(x)) != null)
            {
                var errors = result.Errors.ToList();

                errors.Add("Имя пользователя содержит недопустимые слова");
                result = new IdentityResult(errors);
            }

            if (emailBannder.FirstOrDefault(x => user.Email.ToLower().Contains(x)) != null)
            {
                var errors = result.Errors.ToList();

                errors.Add("Данный e-mail домен находится в спам зоне");
                result = new IdentityResult(errors);
            }

            return result;
        }
    }
}