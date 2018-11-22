using System.Reflection;

namespace SciFiReviewsApi.Repository
{
    public class TypeHelperService : ITypeHelperService
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            foreach (var field in fields.Split(","))
            {
                var propertyName = field.Trim();

                var propertyInfo = typeof(T).GetProperty(
                    propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
