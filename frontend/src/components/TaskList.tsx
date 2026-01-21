import React from 'react';
import type { Task } from '../types';
import { TaskItem } from './TaskItem';

interface TaskListProps {
    tasks: Task[];
    onToggleTask: (id: string, isCompleted: boolean) => void;
    onUpdateTask: (id: string, updates: Partial<Task>) => void;
    onDeleteTask: (id: string) => void;
}

export const TaskList: React.FC<TaskListProps> = ({ tasks, onToggleTask, onUpdateTask, onDeleteTask }) => {
    if (tasks.length === 0) {
        return <div className="empty-state">No tasks yet. Add one above!</div>;
    }

    return (
        <ul className="task-list">
            {tasks.map((task) => (
                <TaskItem 
                    key={task.id} 
                    task={task} 
                    onToggle={onToggleTask}
                    onUpdate={onUpdateTask}
                    onDelete={onDeleteTask}
                />
            ))}
        </ul>
    );
};
