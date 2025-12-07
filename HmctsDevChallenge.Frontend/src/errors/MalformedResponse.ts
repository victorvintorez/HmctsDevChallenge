import {z} from 'zod';

export class MalformedResponseError extends Error {
	constructor(
		data: object,
		expected: z.ZodTypeAny,
		expectedName?: string
	) {
		super(`Malformed ${expectedName} Response:
		Found: ${JSON.stringify(data)}
		Expected: ${JSON.stringify(z.toJSONSchema(expected))}`);
	}
}