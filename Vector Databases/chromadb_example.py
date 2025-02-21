from chromadb import ChromaClient

# Initialize the client
client = ChromaClient()

# Create a collection
collection = client.create_collection(name="example_collection", dimension=128)

# Insert vectors
collection.insert([
    {"id": "vector_id_1", "vector": [0.1, 0.2, 0.3, ..., 0.128]},
    {"id": "vector_id_2", "vector": [0.2, 0.3, 0.4, ..., 0.129]},
    # More vectors
])

# Query the collection
results = collection.query(
    query_vector=[0.1, 0.2, 0.3, ..., 0.128],
    top_k=5
)

# Print the results
print(results)
