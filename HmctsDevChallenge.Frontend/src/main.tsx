import {routeTree} from "./routeTree.gen";
import {createRouter, RouterProvider} from "@tanstack/react-router";
import {QueryClient, QueryClientProvider} from "@tanstack/react-query";
import {createRoot} from "react-dom/client";
import {StrictMode} from "react";

export const queryClient = new QueryClient()

const router = createRouter({
	routeTree,
	context: {
		queryClient
	},
	defaultPreload: "intent",
	defaultPreloadStaleTime: 0
})

declare module "@tanstack/react-router" {
	interface Register {
		router: typeof router;
	}
}

createRoot(document.getElementById('root')!).render(
	<StrictMode>
		<QueryClientProvider client={queryClient}>
			<RouterProvider router={router}/>
		</QueryClientProvider>
	</StrictMode>
)