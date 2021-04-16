using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Properties.Gateway.Config
{
    public class UrlsConfig
    {
        public string PropertiesService { get; set; }

        public string PropertiesGrpc { get; set; }

        public class LibrosOperations
        {
            public static string Get() => $"/api/v1/Libros/Get";

            public static string GetById(int id) => $"/api/v1/Libros/Get/{id}";

            public static string GetByEditorial(int id) => $"/api/v1/Libros/GetByEditorial/{id}";

            public static string Create() => $"/api/v1/Libros/Create";

            public static string Update() => $"/api/v1/Libros/Update";

            public static string Delete(int id) => $"/api/v1/Libros/Delete/{id}";
        }

        public class PropertyOperations
        {
            public static string Get() => $"/api/v1/Properties/Get";

            public static string GetById(Guid id) => $"/api/v1/Properties/Get/{id}";

            public static string Create() => $"/api/v1/Properties/Create";

            public static string Update() => $"/api/v1/Properties/Update";

            public static string Delete(Guid id) => $"/api/v1/Properties/Delete/{id}";
        }

        public class EditorialesOperations
        {
            public static string Get() => $"/api/v1/Editoriales/Get";

            public static string GetById(int id) => $"/api/v1/Editoriales/Get/{id}";

            public static string Create() => $"/api/v1/Editoriales/Create";

            public static string Update() => $"/api/v1/Editoriales/Update";

            public static string Delete(int id) => $"/api/v1/Editoriales/Delete/{id}";
        }
    }

}
