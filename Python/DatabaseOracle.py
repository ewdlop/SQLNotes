from qiskit import QuantumCircuit, QuantumRegister, ClassicalRegister
from qiskit.circuit.library import GroverOperator
from qiskit.algorithms import Grover
from qiskit.primitives import Sampler
import numpy as np

class DatabaseOracle:
    def __init__(self, database_records, target_value, key_size):
        """
        Initialize database oracle for Grover's algorithm
        
        Args:
            database_records (list): List of tuples (id, value) representing database
            target_value: Value to search for
            key_size (int): Number of qubits needed for surrogate key
        """
        self.database = database_records
        self.target = target_value
        self.key_size = key_size
        self.record_count = len(database_records)
        
    def create_oracle_circuit(self) -> QuantumCircuit:
        """Create quantum circuit for database oracle"""
        # Create quantum registers
        qr_key = QuantumRegister(self.key_size, 'key')
        qr_value = QuantumRegister(32, 'value')  # Assuming 32-bit values
        qr_ancilla = QuantumRegister(1, 'ancilla')
        
        # Create quantum circuit
        qc = QuantumCircuit(qr_key, qr_value, qr_ancilla)
        
        # Implement database lookup
        def database_lookup(qc, key_qubits, value_qubits):
            """
            Implement reversible database lookup
            Maps: |k⟩|0⟩ → |k⟩|v_k⟩ where v_k is the value for key k
            """
            for record_id, value in self.database:
                # Convert record_id to binary string
                key_str = format(record_id, f'0{self.key_size}b')
                
                # Control qubits for this record
                ctrl_qubits = []
                for i, bit in enumerate(key_str):
                    if bit == '1':
                        qc.x(key_qubits[i])
                        ctrl_qubits.append(key_qubits[i])
                
                # Apply controlled-X gates to set value bits
                value_str = format(value, '032b')
                for i, bit in enumerate(value_str):
                    if bit == '1':
                        qc.mcx(ctrl_qubits, value_qubits[i])
                
                # Uncompute key transformations
                for i, bit in enumerate(key_str):
                    if bit == '1':
                        qc.x(key_qubits[i])
        
        # Implement value comparison
        def compare_value(qc, value_qubits, target_value, ancilla):
            """Compare value with target and set ancilla"""
            # Convert target to binary
            target_str = format(self.target, '032b')
            
            # XOR value bits with target
            for i, bit in enumerate(target_str):
                if bit == '0':
                    qc.x(value_qubits[i])
            
            # Multi-controlled NOT to ancilla
            qc.mcx(value_qubits, ancilla)
            
            # Uncompute XORs
            for i, bit in enumerate(target_str):
                if bit == '0':
                    qc.x(value_qubits[i])
        
        # Implement full oracle
        database_lookup(qc, qr_key, qr_value)
        compare_value(qc, qr_value, self.target, qr_ancilla[0])
        # Uncompute database lookup for reversibility
        database_lookup(qc, qr_key, qr_value)
        
        return qc

def grover_database_search(database_records, target_value, key_size):
    """
    Perform Grover's search on database to find key for target value
    
    Args:
        database_records (list): List of (id, value) tuples
        target_value: Value to search for
        key_size (int): Number of bits in surrogate key
    
    Returns:
        int: Found key if successful
    """
    # Create oracle
    oracle = DatabaseOracle(database_records, target_value, key_size)
    oracle_circuit = oracle.create_oracle_circuit()
    
    # Create main circuit
    qr_key = QuantumRegister(key_size, 'key')
    cr_key = ClassicalRegister(key_size, 'output')
    qc = QuantumCircuit(qr_key, cr_key)
    
    # Initialize superposition
    qc.h(qr_key)
    
    # Calculate optimal number of iterations
    n = key_size
    N = 2**n
    num_iterations = int(np.pi/4 * np.sqrt(N))
    
    # Apply Grover iterations
    for _ in range(num_iterations):
        # Oracle
        qc.compose(oracle_circuit, qubits=qr_key, inplace=True)
        
        # Diffusion operator
        qc.h(qr_key)
        qc.x(qr_key)
        qc.h(qr_key[-1])
        qc.mcx(qr_key[:-1], qr_key[-1])
        qc.h(qr_key[-1])
        qc.x(qr_key)
        qc.h(qr_key)
    
    # Measure
    qc.measure(qr_key, cr_key)
    
    # Run on simulator
    sampler = Sampler()
    job = sampler.run(qc)
    result = job.result()
    
    # Process results
    counts = result.quasi_dists[0]
    most_frequent = max(counts.items(), key=lambda x: x[1])[0]
    
    # Convert binary string to integer
    found_key = int(most_frequent, 2)
    
    # Verify result
    for record_id, value in database_records:
        if record_id == found_key and value == target_value:
            return found_key
    return None

# Example usage
def main():
    # Example database with surrogate keys
    database = [
        (1, 42),    # ID: 1, Value: 42
        (2, 17),    # ID: 2, Value: 17
        (3, 73),    # ID: 3, Value: 73
        (4, 29),    # ID: 4, Value: 29
        (5, 55)     # ID: 5, Value: 55
    ]
    
    target_value = 73  # Value to search for
    key_size = 3       # Enough bits to represent all IDs
    
    print(f"Searching for record with value: {target_value}")
    print(f"Database size: {len(database)} records")
    print(f"Key size: {key_size} bits")
    
    # Run Grover's algorithm
    result = grover_database_search(database, target_value, key_size)
    
    if result:
        print(f"Found record with ID: {result}")
    else:
        print("Record not found")

if __name__ == "__main__":
    main()
