import { useEffect, useState } from 'react';
import api from '../api/client';

interface FormData {
    id: string;
    subject: string;
    description?: string | null;
    dueDate?: string | null;
    priority?: number | null;
    critical?: boolean | null;
    createdAt: string;
    updatedAt?: string | null;
    createdBy: string;
    updatedBy?: string | null;
    active?: boolean;
}

interface FormDataList {
    results: FormData[];
    pageNumber: number;
    pageSize: number;
    totalRecords: number;
}

export default function FormList() {
    const [forms, setForms] = useState<FormData[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [subjectFilter, setSubjectFilter] = useState('');

    const loadForms = async (filter?: string) => {
        setLoading(true);
        setError(null);

        try {
            const params: any = {
                page: 1,
                pageSize: 50,
            };

            if (filter) {
                params.subjectFilter = filter;
            }

            const response = await api.get<FormDataList>('/api/v1/forms', { params });
            setForms(response.data.results || []);
        } catch (err: any) {
            console.error(err);
            setError(
                err.response?.data?.message ||
                err.response?.data?.title ||
                'Failed to load forms'
            );
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadForms();
    }, []);

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        loadForms(subjectFilter);
    };

    return (
        <div style={{ maxWidth: '1000px', margin: '2rem auto', padding: '0 1rem' }}>
            <h1>Forms</h1>

            {/* Search */}
            <form onSubmit={handleSearch} style={{ marginBottom: '1.5rem', display: 'flex', gap: '0.5rem' }}>
                <input
                    type="text"
                    placeholder="Filter by subject..."
                    value={subjectFilter}
                    onChange={(e) => setSubjectFilter(e.target.value)}
                    style={{ flex: 1, padding: '0.5rem' }}
                />
                <button type="submit" style={{ padding: '0.5rem 1rem' }}>
                    Search
                </button>
                <button
                    type="button"
                    onClick={() => {
                        setSubjectFilter('');
                        loadForms();
                    }}
                    style={{ padding: '0.5rem 1rem' }}
                >
                    Clear
                </button>
            </form>

            {/* Loading / Error */}
            {loading && <p>Loading...</p>}
            {error && <p style={{ color: 'red' }}>{error}</p>}

            {/* Table */}
            {!loading && !error && (
                <>
                    <p>{forms.length} record(s) found</p>

                    <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                        <thead>
                            <tr style={{ backgroundColor: '#f0f0f0', textAlign: 'left' }}>
                                <th style={{ padding: '0.75rem', border: '1px solid #ddd' }}>Subject</th>
                                <th style={{ padding: '0.75rem', border: '1px solid #ddd' }}>Priority</th>
                                <th style={{ padding: '0.75rem', border: '1px solid #ddd' }}>Critical</th>
                                <th style={{ padding: '0.75rem', border: '1px solid #ddd' }}>Due Date</th>
                                <th style={{ padding: '0.75rem', border: '1px solid #ddd' }}>Created By</th>
                                <th style={{ padding: '0.75rem', border: '1px solid #ddd' }}>Created At</th>
                            </tr>
                        </thead>
                        <tbody>
                            {forms.map((form) => (
                                <tr key={form.id}>
                                    <td style={{ padding: '0.75rem', border: '1px solid #ddd' }}>
                                        {form.subject}
                                    </td>
                                    <td style={{ padding: '0.75rem', border: '1px solid #ddd' }}>
                                        {form.priority ?? '-'}
                                    </td>
                                    <td style={{ padding: '0.75rem', border: '1px solid #ddd' }}>
                                        {form.critical ? 'Yes' : 'No'}
                                    </td>
                                    <td style={{ padding: '0.75rem', border: '1px solid #ddd' }}>
                                        {form.dueDate ? new Date(form.dueDate).toLocaleDateString() : '-'}
                                    </td>
                                    <td style={{ padding: '0.75rem', border: '1px solid #ddd' }}>
                                        {form.createdBy}
                                    </td>
                                    <td style={{ padding: '0.75rem', border: '1px solid #ddd' }}>
                                        {new Date(form.createdAt).toLocaleString()}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>

                    {forms.length === 0 && <p>No forms found.</p>}
                </>
            )}
        </div>
    );
}