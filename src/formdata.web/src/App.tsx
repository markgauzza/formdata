import { Routes, Route, Link } from 'react-router-dom';
import FormList from './pages/FormList';
import CreateForm from './pages/Form';

function App() {
    return (
        <div>
            {/* Navigation */}
            <nav style={{ padding: '1rem', backgroundColor: '#eee', marginBottom: '1rem' }}>
                <Link to="/" style={{ marginRight: '1rem' }}>
                    List Forms
                </Link>
                <Link to="/create">
                    Create Form
                </Link>
            </nav>

            {/* Pages */}
            <Routes>
                <Route path="/" element={<FormList />} />
                <Route path="/create" element={<CreateForm />} />
            </Routes>
        </div>
    );
}

export default App;