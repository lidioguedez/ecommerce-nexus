-- Initialize E-commerce Nexus Database
CREATE DATABASE IF NOT EXISTS ecommerce_nexus;

-- Create separate databases for different services
CREATE DATABASE IF NOT EXISTS product_service;
CREATE DATABASE IF NOT EXISTS order_service;
CREATE DATABASE IF NOT EXISTS user_service;

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE ecommerce_nexus TO postgres;
GRANT ALL PRIVILEGES ON DATABASE product_service TO postgres;
GRANT ALL PRIVILEGES ON DATABASE order_service TO postgres;
GRANT ALL PRIVILEGES ON DATABASE user_service TO postgres;