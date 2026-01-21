import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../services/api';
import { logger } from '../utils/logger';
import type { Task } from '../types';

const TASKS_QUERY_KEY = ['tasks'];

export const useTasks = () => {
    const queryClient = useQueryClient();

    // Fetch tasks
    const { 
        data: tasks = [], 
        isLoading: loading, 
        error,
        refetch: refreshTasks 
    } = useQuery({
        queryKey: TASKS_QUERY_KEY,
        queryFn: async () => {
            logger.info('Fetching tasks');
            const data = await api.getTasks();
            logger.info('Tasks loaded successfully', data.length);
            return data;
        },
    });

    // Create task mutation
    const createTaskMutation = useMutation({
        mutationFn: ({ title, description }: { title: string; description?: string }) => 
            api.createTask(title, description),
        onSuccess: (newTask) => {
            queryClient.setQueryData<Task[]>(TASKS_QUERY_KEY, (old = []) => [...old, newTask]);
            logger.info('Task added successfully', newTask.id);
        },
        onError: (err) => {
            const message = 'Failed to add task';
            logger.error(message, err);
            alert(message);
        },
    });

    // Update task mutation
    const updateTaskMutation = useMutation({
        mutationFn: ({ id, updates }: { id: string; updates: Partial<Task> }) => 
            api.updateTask(id, updates),
        onSuccess: (updatedTask) => {
            queryClient.setQueryData<Task[]>(TASKS_QUERY_KEY, (old = []) =>
                old.map(t => t.id === updatedTask.id ? updatedTask : t)
            );
            logger.info('Task updated successfully', updatedTask.id);
        },
        onError: (err) => {
            const message = 'Failed to update task';
            logger.error(message, err);
            alert(message);
        },
    });

    // Delete task mutation
    const deleteTaskMutation = useMutation({
        mutationFn: (id: string) => api.deleteTask(id),
        onSuccess: (_, id) => {
            queryClient.setQueryData<Task[]>(TASKS_QUERY_KEY, (old = []) =>
                old.filter(t => t.id !== id)
            );
            logger.info('Task deleted successfully', id);
        },
        onError: (err) => {
            const message = 'Failed to delete task';
            logger.error(message, err);
            alert(message);
        },
    });

    const handleAddTask = async (title: string, description?: string) => {
        await createTaskMutation.mutateAsync({ title, description });
    };

    const handleToggleTask = async (id: string, isCompleted: boolean) => {
        await updateTaskMutation.mutateAsync({ id, updates: { isCompleted } });
    };

    const handleUpdateTask = async (id: string, updates: Partial<Task>) => {
        await updateTaskMutation.mutateAsync({ id, updates });
    };

    const handleDeleteTask = async (id: string) => {
        await deleteTaskMutation.mutateAsync(id);
    };

    return {
        tasks,
        loading,
        error: error ? 'Failed to load tasks' : null,
        handleAddTask,
        handleToggleTask,
        handleUpdateTask,
        handleDeleteTask,
        refreshTasks
    };
};
