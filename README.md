# Gazella - Article Data Service #

Este repositorio contiene el servicio gRPC de articulos del sistema Gazella

## Stack tecnologico ##

* ASP.NET Core + gRPC Core
* Base de Datos MongoDB
* Entity Framework Core + MongoDB Driver

## Ejecutando el proyecto ##

Cree un archivo .env con la siguiente estructura:

```text
MONGO_DB=[db name]

MONGO_ROOT=[admin name]
MONGO_ROOT_PASS=[pass]

MONGO_USER=[user name]
MONGO_USER_PASS=[pass]
```

Una vez definidas las variables de entorno levante los contenedores:

```bash
docker compose up --build
```

Para probar el proyecto conviene tener datos precargados en categorias, conectese al contenedor de la base de datos:

```bash
docker exec -it articles_mongodb mongosh 'mongodb://[usuario]:[pass]@localhost:27017/[db name]?authSource=admin'
```

**Nota**: *?authSource=admin*, solo es necesario si decide crear la colección con el usuario admin. Si lo hace con el usuario normal puede remover *?authSource*

Aquí tiene una muestra de datos que puede copiar y pegar en el terminal para poblar la base de datos:

```text
db.createCollection("categories", {
   validator: {
      $jsonSchema: {
         bsonType: "object",
         required: ["_id", "name"],
         properties: {
            _id: {
               bsonType: "string",
               description: "ID must be a string (UUIDv4) and is required"
            },
            name: {
               bsonType: "string",
               maxLength: 64,
               description: "Name must be a string of up to 64 characters and is required"
            }
         }
      }
   }
});

db.categories.insertMany([
  {
    _id: "b3235e5a-5f44-494c-a330-3e8a3ad3b145",
    name: "Biodiversidad"
  },
  {
    _id: "57a345f3-1d14-4dc5-b2a0-8f140e2162f5",
    name: "Areas protegidas"
  },
  {
    _id: "75d06355-6891-4a1e-ba5a-12d7863a754e",
    name: "Cambio climatico"
  },
  {
    _id: "efe49e65-1ad6-41ee-bc6b-40f395e00f0d",
    name: "Flora y fauna"
  }
]);
```
