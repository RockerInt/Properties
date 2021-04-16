# Books

Este es un proyecto de prueba para manejo de Propiedades, Dueños, etc. que implementa *.NET 5.0, SQL Server 2019 y Docker*.


## Build & Test

Se debe clonar el repositorio y en la raíz de proyecto ejecutar:

> docker-compose up -d

O
Luego se debe conectar a la base de datos y alguno de los archivos para restaura la DB *DB/Books/Restore*

Estos son los servicios que corren en el docker-compose:

- Properties(Microservices): http://localhost:52964, https://localhost:44304
- Gateway(API): http://localhost:50412, https://localhost:44333
- SqlServer: "Server=localhost;User Id=sa;Password=12345678a"

## Repository

https://github.com/RockerInt/Properties

## License
Code released under the [MIT license](https://opensource.org/licenses/MIT).
