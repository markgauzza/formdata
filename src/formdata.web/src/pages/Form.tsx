import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useState } from 'react';
import api from '../api/client';

// Validation schema (matches your API rules)
const createFormSchema = z.object({
    subject: z.string().min(1, 'Subject is required').max(200, 'Max 200 characters'),
    description: z.string().optional(),
    dueDate: z.string().optional(), // we'll send as ISO string
    priority: z.coerce.number().min(1).max(10).optional().or(z.literal('')),
    critical: z.boolean().optional(),
});

type CreateFormValues = z.infer<typeof createFormSchema>;

export default function CreateForm() {
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<CreateFormValues>({
        resolver: zodResolver(createFormSchema),
        defaultValues: {
            subject: '',
            description: '',
            dueDate: '',
            priority: '',
            critical: false,
        },
    });

    const onSubmit = async (data: CreateFormValues) => {
        setIsSubmitting(true);
        setSuccessMessage(null);
        setErrorMessage(null);

        try {
            // Prepare payload for your API
            const payload = {
                subject: data.subject,
                description: data.description || null,
                dueDate: data.dueDate ? new Date(data.dueDate).toISOString() : null,
                priority: data.priority === '' ? null : Number(data.priority),
                critical: data.critical ?? false,
                // CreatedBy is usually set on the server from the JWT
            };

            const response = await api.post('/api/forms', payload);

            setSuccessMessage(`Form created successfully! ID: ${response.data.id}`);
            reset(); // clear the form
        } catch (err: any) {
            console.error(err);
            const message =
                err.response?.data?.message ||
                err.response?.data?.title ||
                'Failed to create form';
            setErrorMessage(message);
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div style={{ maxWidth: '600px', margin: '2rem auto', padding: '0 1rem' }}>
            <h1>Create New Form</h1>

            <form onSubmit={handleSubmit(onSubmit)} style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>

                {/* Subject */}
                <div>
                    <label htmlFor="subject">Subject *</label>
                    <input
                        id="subject"
                        {...register('subject')}
                        placeholder="Enter subject"
                        style={{ width: '100%', padding: '0.5rem' }}
                    />
                    {errors.subject && (
                        <p style={{ color: 'red', margin: '0.25rem 0 0' }}>{errors.subject.message}</p>
                    )}
                </div>

                {/* Description */}
                <div>
                    <label htmlFor="description">Description</label>
                    <textarea
                        id="description"
                        {...register('description')}
                        rows={4}
                        placeholder="Optional description"
                        style={{ width: '100%', padding: '0.5rem' }}
                    />
                </div>

                {/* Due Date */}
                <div>
                    <label htmlFor="dueDate">Due Date</label>
                    <input
                        id="dueDate"
                        type="date"
                        {...register('dueDate')}
                        style={{ width: '100%', padding: '0.5rem' }}
                    />
                </div>

                {/* Priority */}
                <div>
                    <label htmlFor="priority">Priority (1-10)</label>
                    <input
                        id="priority"
                        type="number"
                        min={1}
                        max={10}
                        {...register('priority')}
                        style={{ width: '100%', padding: '0.5rem' }}
                    />
                    {errors.priority && (
                        <p style={{ color: 'red', margin: '0.25rem 0 0' }}>{errors.priority.message}</p>
                    )}
                </div>

                {/* Critical */}
                <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                    <input id="critical" type="checkbox" {...register('critical')} />
                    <label htmlFor="critical">Critical</label>
                </div>

                {/* Submit */}
                <button
                    type="submit"
                    disabled={isSubmitting}
                    style={{
                        padding: '0.75rem',
                        backgroundColor: isSubmitting ? '#ccc' : '#0078d4',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: isSubmitting ? 'not-allowed' : 'pointer',
                    }}
                >
                    {isSubmitting ? 'Creating...' : 'Create Form'}
                </button>
            </form>

            {/* Feedback messages */}
            {successMessage && (
                <p style={{ color: 'green', marginTop: '1rem' }}>{successMessage}</p>
            )}
            {errorMessage && (
                <p style={{ color: 'red', marginTop: '1rem' }}>{errorMessage}</p>
            )}
        </div>
    );
}