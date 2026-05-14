#!/bin/bash
set -e

echo "Creating user for database: $MONGO_DB"

mongosh -u "$MONGO_INITDB_ROOT_USERNAME" -p "$MONGO_INITDB_ROOT_PASSWORD" --authenticationDatabase admin <<EOF
use $MONGO_DB

db.createUser({
  user: '$MONGO_USER',
  pwd:  '$MONGO_USER_PASS',
  roles: [{
    role: 'readWrite',
    db: '$MONGO_DB'
  }]
})
EOF

echo "User created successfully."