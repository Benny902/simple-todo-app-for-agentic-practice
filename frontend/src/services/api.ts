import axios from 'axios';
import { config } from '../config';
import type { Task } from '../types';

const axiosInstance = axios.create({
    baseURL: config.API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

export const api = {
    getTasks: async (): Promise<Task[]> => {
        const { data } = await axiosInstance.get<Task[]>('/api/tasks');
        return data;
    },

    createTask: async (title: string, description?: string): Promise<Task> => {
        const { data } = await axiosInstance.post<Task>('/api/tasks', { title, description });
        return data;
    },

    updateTask: async (id: string, updates: Partial<Task>): Promise<Task> => {
        const { data } = await axiosInstance.patch<Task>(`/api/tasks/${id}`, updates);
        return data;
    },

    deleteTask: async (id: string): Promise<void> => {
        await axiosInstance.delete(`/api/tasks/${id}`);
    }
};
