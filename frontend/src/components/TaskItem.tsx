import React, { useState } from 'react';
import type { Task } from '../types';

interface TaskItemProps {
    task: Task;
    onToggle: (id: string, isCompleted: boolean) => void;
    onUpdate: (id: string, updates: Partial<Task>) => void;
    onDelete: (id: string) => void;
}

export const TaskItem: React.FC<TaskItemProps> = ({ task, onToggle, onUpdate, onDelete }) => {
    const [isEditing, setIsEditing] = useState(false);
    const [editTitle, setEditTitle] = useState(task.title);
    const [editDescription, setEditDescription] = useState(task.description || '');

    const handleSave = () => {
        if (editTitle.trim()) {
            onUpdate(task.id, { 
                title: editTitle, 
                description: editDescription || undefined 
            });
            setIsEditing(false);
        }
    };

    const handleCancel = () => {
        setEditTitle(task.title);
        setEditDescription(task.description || '');
        setIsEditing(false);
    };

    if (isEditing) {
        return (
            <li className="task-item editing">
                <div className="task-edit-form">
                    <input
                        type="text"
                        value={editTitle}
                        onChange={(e) => setEditTitle(e.target.value)}
                        className="task-edit-input"
                        autoFocus
                    />
                    <textarea
                        value={editDescription}
                        onChange={(e) => setEditDescription(e.target.value)}
                        className="task-edit-textarea"
                        placeholder="Description (optional)"
                        rows={2}
                    />
                    <div className="task-edit-buttons">
                        <button onClick={handleSave} className="btn-save">Save</button>
                        <button onClick={handleCancel} className="btn-cancel">Cancel</button>
                    </div>
                </div>
            </li>
        );
    }

    return (
        <li className={`task-item ${task.isCompleted ? 'completed' : ''}`}>
            <div className="task-content">
                <input 
                    type="checkbox" 
                    checked={task.isCompleted} 
                    onChange={(e) => onToggle(task.id, e.target.checked)}
                    className="task-checkbox"
                />
                <div className="task-text">
                    <span className="task-title">{task.title}</span>
                    {task.description && (
                        <p className="task-description">{task.description}</p>
                    )}
                </div>
            </div>
            <div className="task-actions">
                <button onClick={() => setIsEditing(true)} className="btn-edit">Edit</button>
                <button onClick={() => onDelete(task.id)} className="btn-delete">Delete</button>
            </div>
        </li>
    );
};
