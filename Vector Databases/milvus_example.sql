-- Create a collection
CREATE TABLE example_collection (
    id INT PRIMARY KEY,
    vector FLOAT[] VECTOR(128)
);

-- Insert vectors
INSERT INTO example_collection (id, vector)
VALUES
(1, [0.1, 0.2, 0.3, ..., 0.128]),
(2, [0.2, 0.3, 0.4, ..., 0.129]);

-- Query vectors
SELECT id
FROM example_collection
ORDER BY vector_distance(vector, [0.1, 0.2, 0.3, ..., 0.128])
LIMIT 5;
