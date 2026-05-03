import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import type { TaskPriority } from '../types';

interface TaskInputProps {
    onAddTask: (title: string, priority: TaskPriority) => Promise<void>;
}

export const TaskInput: React.FC<TaskInputProps> = ({ onAddTask }) => {
    const { t } = useTranslation();
    const [title, setTitle] = useState('');
    const [priority, setPriority] = useState<TaskPriority>('medium');
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!title.trim()) return;

        setIsSubmitting(true);
        try {
            await onAddTask(title, priority);
            setTitle('');
            setPriority('medium');
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
                placeholder={t('tasks.placeholder')}
                disabled={isSubmitting}
            />
            <label className="priority-field">
                <span>{t('tasks.priorityLabel')}</span>
                <select
                    value={priority}
                    onChange={(e) => setPriority(e.target.value as TaskPriority)}
                    disabled={isSubmitting}
                >
                    <option value="low">{t('tasks.priority.low')}</option>
                    <option value="medium">{t('tasks.priority.medium')}</option>
                    <option value="high">{t('tasks.priority.high')}</option>
                </select>
            </label>
            <button type="submit" disabled={isSubmitting || !title.trim()}>
                {t('tasks.addButton')}
            </button>
        </form>
    );
};
