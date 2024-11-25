// Base URL for the API
const baseURL = 'https://localhost:5003/api';

// Function to get a Title from the API
export const fetchTitle = async () => {
    try {
        const response = await fetch(`${baseURL}/Title/tt1375666`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log('API Response:', data);
        return data;
    } catch (error) {
        console.error('Error fetching title:', error);
        throw error;
    }
};

// Function to post data (Just placeholder)
export const createUser = async (userData) => {
    try {
        const response = await fetch(`${baseURL}/******?`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(userData),
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error creating user:', error);
        throw error;
    }
};
