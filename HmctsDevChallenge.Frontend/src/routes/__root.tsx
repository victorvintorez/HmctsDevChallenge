import * as React from "react";
import {useEffect} from "react";
import {createRootRouteWithContext, Outlet, useLocation, useRouter} from "@tanstack/react-router";
import {type QueryClient, useQueryErrorResetBoundary} from "@tanstack/react-query";
import {TanStackRouterDevtoolsPanel} from "@tanstack/react-router-devtools";
import {ReactQueryDevtoolsPanel} from "@tanstack/react-query-devtools";
import Layout from "../components/layout/Layout.tsx";
import {TanStackDevtools} from "@tanstack/react-devtools";
import {FormDevtoolsPanel} from "@tanstack/react-form-devtools";

const RootComponent: React.FC = () => {
	return (
		<>
			<Layout>
				<Outlet/>
			</Layout>
			<TanStackDevtools
				plugins={[
					{
						name: 'TanStack Query',
						render: <ReactQueryDevtoolsPanel/>,
						defaultOpen: true
					},
					{
						name: 'TanStack Router',
						render: <TanStackRouterDevtoolsPanel/>,
						defaultOpen: false
					},
					{
						name: 'TanStack Form',
						render: <FormDevtoolsPanel/>,
						defaultOpen: false
					}
				]}
			/>
		</>
	)
}

const NotFoundComponent: React.FC = () => {
	const location = useLocation()

	return (
		<>
			<p>404 - Not Found</p>
			<p>{location.pathname}</p>
		</>
	)
}

interface ErrorComponentProps {
	error: Error
}

const ErrorComponent: React.FC<ErrorComponentProps> = ({error}) => {
	const router = useRouter()
	const queryErrorResetBoundary = useQueryErrorResetBoundary()

	useEffect(() => {
		queryErrorResetBoundary.reset()
	}, [queryErrorResetBoundary])

	return (
		<>
			<p>{error.message}</p>
			<button onClick={() => void (async () => await router.invalidate())()}>Reload</button>
		</>
	)
}

export const Route = createRootRouteWithContext<{ queryClient: QueryClient }>()({
	component: RootComponent,
	notFoundComponent: NotFoundComponent,
	errorComponent: ({error}) => <ErrorComponent error={error}/>
})