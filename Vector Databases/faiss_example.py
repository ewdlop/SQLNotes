import faiss
import numpy as np

# Create a random set of vectors
d = 128  # dimension
nb = 10000  # database size
nq = 100  # number of queries

xb = np.random.random((nb, d)).astype('float32')
xq = np.random.random((nq, d)).astype('float32')

# Build the index
index = faiss.IndexFlatL2(d)
index.add(xb)

# Query the index
D, I = index.search(xq, k=5)  # search for the 5 nearest neighbors

# Print the results
print(I)
print(D)
