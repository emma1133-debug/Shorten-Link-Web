-- Tạo database nếu chưa tồn tại
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'urlshortenerdb') THEN
        CREATE DATABASE urlshortenerdb;
    END IF;
END $$;

-- Tạo user nếu chưa có
DO $$ 
BEGIN
   IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'appuser') THEN
      CREATE ROLE appuser WITH LOGIN PASSWORD 'secret123';
      ALTER ROLE appuser CREATEDB;
   END IF;
END $$;

-- Gán quyền cho user
GRANT ALL PRIVILEGES ON DATABASE urlshortenerdb TO appuser;

-- Chuyển sang database chính để tạo bảng
\connect urlshortenerdb

-- Tạo bảng UrlMappings nếu chưa tồn tại
CREATE TABLE IF NOT EXISTS UrlMappings (
    Id SERIAL PRIMARY KEY,
    ShortKey VARCHAR(10) UNIQUE NOT NULL,
    OriginalUrl TEXT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Title TEXT
);

-- Gán quyền đầy đủ cho user
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO appuser;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO appuser;
GRANT ALL PRIVILEGES ON ALL FUNCTIONS IN SCHEMA public TO appuser;

ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO appuser;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO appuser;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON FUNCTIONS TO appuser;
