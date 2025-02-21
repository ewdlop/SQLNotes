import pinecone

# Initialize connection to Pinecone
pinecone.init(api_key="your-api-key", environment="us-west1-gcp")

# Create an index
pinecone.create_index("example-index", dimension=128)

# Connect to the index
index = pinecone.Index("example-index")

# Upsert vectors
index.upsert([
    ("vector_id_1", [0.1, 0.2, 0.3, ..., 0.128]),
    ("vector_id_2", [0.2, 0.3, 0.4, ..., 0.129]),
    # More vectors
])

# Query the index
query_response = index.query(
    queries=[[0.1, 0.2, 0.3, ..., 0.128]],
    top_k=5
)

# Print the results
print(query_response)
