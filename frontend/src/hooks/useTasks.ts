import { useState, useEffect } from 'react';
import { api } from '../services/api';
import { logger } from '../utils/logger';
import i18n from '../i18n';
import type { Task, TaskPriority } from '../types';

const priorityRank: Record<TaskPriority, number> = {
    high: 0,
    medium: 1,
    low: 2,
};

const sortTasks = (tasks: Task[]) => {
    return [...tasks].sort((a, b) => {
        if (a.isCompleted !== b.isCompleted) {
            return Number(a.isCompleted) - Number(b.isCompleted);
        }

        const byPriority = priorityRank[a.priority] - priorityRank[b.priority];
        if (byPriority !== 0) {
            return byPriority;
        }

        return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
    });
};

export const useTasks = () => {
    const [tasks, setTasks] = useState<Task[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const loadTasks = async () => {
        try {
            setLoading(true);
            const data = await api.getTasks();
            setTasks(sortTasks(data));
            setError(null);
            logger.info('Tasks loaded successfully', data.length);
        } catch (err) {
            const message = i18n.t('errors.loadFailed');
            setError(message);
            logger.error(message, err);
        } finally {
            setLoading(false);
        }
    };

    const handleAddTask = async (title: string, priority: TaskPriority) => {
        try {
            const newTask = await api.createTask(title, priority);
            setTasks(prev => sortTasks([...prev, newTask]));
            logger.info('Task added successfully', newTask.id);
        } catch (err) {
            const message = i18n.t('errors.addFailed');
            logger.error(message, err);
            alert(message);
        }
    };

    const handleToggleTask = async (id: string, isCompleted: boolean) => {
        try {
            const updatedTask = await api.updateTask(id, { isCompleted });
            setTasks(prev => sortTasks(prev.map(t => t.id === id ? updatedTask : t)));
            logger.info('Task updated successfully', id);
        } catch (err) {
            const message = i18n.t('errors.updateFailed');
            logger.error(message, err);
            alert(message);
        }
    };

    useEffect(() => {
        loadTasks();
    }, []);

    return {
        tasks,
        loading,
        error,
        handleAddTask,
        handleToggleTask,
        refreshTasks: loadTasks
    };
};
