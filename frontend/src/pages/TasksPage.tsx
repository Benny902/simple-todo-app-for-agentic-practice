import { useEffect, useState } from 'react';
import { api } from '../services/api';
import type { Task } from '../types';
import { TaskList } from '../components/TaskList';
import { TaskInput } from '../components/TaskInput';

export const TasksPage = () => {
    const [tasks, setTasks] = useState<Task[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadTasks();
    }, []);

    const loadTasks = async () => {
        try {
            setLoading(true);
            const data = await api.getTasks();
            setTasks(data);
            setError(null);
        } catch (err) {
            setError('Failed to load tasks');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleAddTask = async (title: string) => {
        try {
            const newTask = await api.createTask(title);
            setTasks([...tasks, newTask]);
        } catch (err) {
            console.error(err);
            alert('Failed to add task');
        }
    };

    const handleToggleTask = async (id: string, isCompleted: boolean) => {
        try {
            const updatedTask = await api.updateTask(id, { isCompleted });
            setTasks(tasks.map(t => t.id === id ? updatedTask : t));
        } catch (err) {
            console.error(err);
            alert('Failed to update task');
        }
    };

    return (
        <div className="tasks-page">
            <header className="page-header">
                <h1>Simple Task Manager</h1>
                <p>Simple task management for your day.</p>
            </header>
            
            <section className="input-section">
                <TaskInput onAddTask={handleAddTask} />
            </section>

            <section className="list-section">
                {error && <div className="error-message">{error}</div>}
                {loading ? (
                    <div className="loading-state">Loading tasks...</div>
                ) : (
                    <TaskList tasks={tasks} onToggleTask={handleToggleTask} />
                )}
            </section>
        </div>
    );
};
