import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App.tsx';
import { fetchDevToken } from './api/auth';
import './index.css';

async function startApp() {
    try {
        // Only fetch a new token if we don't already have one
        if (!localStorage.getItem('token')) {
            console.log('Fetching development token...');
            const token = await fetchDevToken();
            localStorage.setItem('token', token);
            console.log('Token stored in localStorage');
        }
    } catch (err) {
        console.error('Failed to fetch development token', err);
    }
    createRoot(document.getElementById('root')!).render(
        <StrictMode>
            <BrowserRouter>
                <App />
            </BrowserRouter>
        </StrictMode>,
    )
}

startApp();

