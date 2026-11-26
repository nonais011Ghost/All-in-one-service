namespace AIOSICT011.Business
{
    // Input checks used by the registration and feedback screens.
    // These checks use basic string operations and loops instead of regular expressions.
    public static class Validator
    {
        public static bool IsRequired(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        // Botswana style phone number: 7 or 8 digits, may start with +267
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;

            string number = phone.Replace(" ", "").Replace("-", "");
            if (number.StartsWith("+267"))
                number = number.Substring(4);

            if (number.Length != 7 && number.Length != 8) return false;

            foreach (char character in number)
            {
                if (!IsDigit(character)) return false;
            }
            return true;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            string address = email.Trim();
            int atPosition = address.IndexOf('@');
            if (atPosition <= 0 || atPosition == address.Length - 1 ||
                atPosition != address.LastIndexOf('@')) return false;

            int dotPosition = address.IndexOf('.', atPosition + 1);
            if (dotPosition <= atPosition + 1 || dotPosition == address.Length - 1) return false;

            foreach (char character in address)
            {
                if (char.IsWhiteSpace(character)) return false;
            }
            return true;
        }

        // 4 to 20 letters, digits or underscores
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            if (username.Length < 4 || username.Length > 20) return false;

            foreach (char character in username)
            {
                if (!IsLetter(character) && !IsDigit(character) && character != '_')
                    return false;
            }
            return true;
        }

        public static bool IsValidPassword(string password)
        {
            return password != null && password.Length >= 6;
        }

        // 5 to 20 letters, digits, - or /
        public static bool IsValidBusinessReg(string reg)
        {
            if (string.IsNullOrWhiteSpace(reg)) return false;

            string registration = reg.Trim();
            if (registration.Length < 5 || registration.Length > 20) return false;

            foreach (char character in registration)
            {
                if (!IsLetter(character) && !IsDigit(character) &&
                    character != '/' && character != '-')
                    return false;
            }
            return true;
        }

        public static bool IsValidRating(int rating)
        {
            return rating >= 1 && rating <= 5;
        }

        private static bool IsLetter(char character)
        {
            return (character >= 'A' && character <= 'Z') ||
                   (character >= 'a' && character <= 'z');
        }

        private static bool IsDigit(char character)
        {
            return character >= '0' && character <= '9';
        }
    }
}
