import React from 'react';
import type { Task } from '../types';

interface TaskListProps {
    tasks: Task[];
}

export const TaskList: React.FC<TaskListProps> = ({ tasks }) => {
    if (tasks.length === 0) {
        return <div className="empty-state">No tasks yet. Add one above!</div>;
    }

    return (
        <ul className="task-list">
            {tasks.map((task) => (
                <li key={task.id} className={`task-item ${task.isCompleted ? 'completed' : ''}`}>
                    <span className="task-title">{task.title}</span>
                </li>
            ))}
        </ul>
    );
};
