import React, { useState } from 'react';

interface TaskInputProps {
    onAddTask: (title: string, description?: string) => Promise<void>;
}

export const TaskInput: React.FC<TaskInputProps> = ({ onAddTask }) => {
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!title.trim()) return;

        setIsSubmitting(true);
        try {
            await onAddTask(title, description || undefined);
            setTitle('');
            setDescription('');
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="task-input">
            <input
                type="text"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="What needs to be done?"
                disabled={isSubmitting}
            />
            <textarea
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Description (optional)"
                disabled={isSubmitting}
                rows={2}
            />
            <button type="submit" disabled={isSubmitting || !title.trim()}>
                Add Task
            </button>
        </form>
    );
};
