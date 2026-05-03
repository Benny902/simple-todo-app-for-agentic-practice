import React from 'react';
import { useTranslation } from 'react-i18next';
import type { Task } from '../types';

interface TaskItemProps {
    task: Task;
    onToggle: (id: string, isCompleted: boolean) => void;
}

export const TaskItem: React.FC<TaskItemProps> = ({ task, onToggle }) => {
    const { t } = useTranslation();

    return (
        <li className={`task-item ${task.isCompleted ? 'completed' : ''}`}>
            <input 
                type="checkbox" 
                checked={task.isCompleted} 
                onChange={(e) => onToggle(task.id, e.target.checked)}
                className="task-checkbox"
            />
            <span className="task-title">{task.title}</span>
            <span className={`priority-badge priority-${task.priority}`}>
                {t(`tasks.priority.${task.priority}`)}
            </span>
        </li>
    );
};
