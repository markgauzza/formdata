import api from './client';

export async function fetchDevToken(): Promise<string> {
    const response = await api.get('/api/auth/token', {
        // optional query params
        params: {
            userId: '1234567890',
            name: 'John Doe',
        },
    });

    return response.data.token;
}