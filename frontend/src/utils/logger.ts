export const logger = {
    info: (message: string, ...args: unknown[]) => {
        if (import.meta.env.DEV) {
            console.log(`[INFO] ${message}`, ...args);
        }
    },
        error: (message: string, ...args: unknown[]) => {
        console.error(`[ERROR] ${message}`, ...args);
    },
    warn: (message: string, ...args: unknown[]) => {
        if (import.meta.env.DEV) {
            console.warn(`[WARN] ${message}`, ...args);
        }
    }
};
