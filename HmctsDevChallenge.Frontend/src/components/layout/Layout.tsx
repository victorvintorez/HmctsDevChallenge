import * as React from "react";
import {type PropsWithChildren} from "react";
import * as GovUK from "govuk-react";


const Layout: React.FC<PropsWithChildren> = ({children}) => {
	return (
		<>
			<GovUK.GlobalStyle/>
			<GovUK.TopNav company={
				<GovUK.TopNav.NavLink href="/">HMCTS</GovUK.TopNav.NavLink>
			} serviceTitle={
				<GovUK.TopNav.NavLink>Caseworker Tasks</GovUK.TopNav.NavLink>
			}/>
			<GovUK.Main>{children}</GovUK.Main>
			<GovUK.Footer meta={
				<GovUK.Footer.MetaCustom>
					Built for the {' '}
					<GovUK.Footer.Link href="https://github.com/hmcts/dts-developer-challenge-junior">
						HMCTS Junior Developer Challenge
					</GovUK.Footer.Link>
				</GovUK.Footer.MetaCustom>
			}/>
		</>
	)
}

export default Layout;